using DroneService.Data;
using DroneService.Data.Entities;
using System.Net.Mail;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using DroneService.Utilities.Options;
using DroneService.Data.Interfaces;
using DroneService.Application.Contracts.Interfaces;

namespace DroneService.Application.Contracts.Services;

// Implementace služby pro práci s emaily (uložení + odeslání)
public class EmailSenderService : IEmailSenderService
{
    private readonly AppDbContext _dbContext;     // databázový kontext
    private readonly SmtpOptions _smtpOptions;    // SMTP konfigurace (host, port, login...)
    private readonly EnvironmentOptions _envOptions; // defaultní email + jméno odesílatele
    private readonly IClock _clock;               // práce s časem

    public EmailSenderService(
        IClock clock,
        AppDbContext appDbContext,
        IOptions<EnvironmentOptions> envOptions,
        IOptions<SmtpOptions> options)
    {
        _dbContext = appDbContext;
        _smtpOptions = options.Value;
        _envOptions = envOptions.Value;
        _clock = clock;
    }

    public async Task AddEmail(
        string subject,
        string body,
        string recipientEmail,
        string? recipientName = null,
        string? fromEmail = null,
        string? fromName = null)
    {
        // fallback na defaultní hodnoty, pokud nejsou předány
        var fromEmailValue = fromEmail ?? _envOptions.SenderEmail;
        var fromNameValue = fromName ?? _envOptions.SenderName;

        // vytvoření entity emailu (uložení do DB fronty)
        var message = new EmailMessage
        {
            Subject = subject,
            Body = body,
            RecipientEmail = recipientEmail,
            RecipientName = recipientName,
            FromEmail = fromEmailValue,
            FromName = fromNameValue,
            Sent = false, // zatím neodesláno
            CreatedAt = _clock.GetCurrentInstant(),
        };

        // uložení do databáze
        _dbContext.Add(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SendEmailAsync()
    {
        // načtení všech emailů, které ještě nebyly odeslány
        var unsentMails = await _dbContext.Emails
            .Where(x => !x.Sent)
            .ToListAsync();

        foreach (var unsent in unsentMails)
        {
            // kontrola odesílatele
            if (string.IsNullOrEmpty(unsent.FromEmail))
            {
                Console.WriteLine($"Skipping email with ID {unsent.Id}: FromEmail is null or empty.");
                continue;
            }

            // kontrola příjemce
            if (string.IsNullOrEmpty(unsent.RecipientEmail))
            {
                Console.WriteLine($"Skipping email with ID {unsent.Id}: RecipientEmail is null or empty.");
                continue;
            }

            // fallback hodnoty pro jména
            var fromName = string.IsNullOrEmpty(unsent.FromName)
                ? _envOptions.SenderName
                : unsent.FromName;

            var recipientName = string.IsNullOrEmpty(unsent.RecipientName)
                ? "Recipient"
                : unsent.RecipientName;

            // vytvoření email zprávy
            using var mail = new MailMessage
            {
                Subject = unsent.Subject,
                Body = unsent.Body,
                IsBodyHtml = true,
                From = new MailAddress(unsent.FromEmail, fromName),
            };

            // přidání příjemce
            mail.To.Add(new MailAddress(unsent.RecipientEmail, recipientName));

            try
            {
                // vytvoření SMTP klienta (MailKit)
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                // připojení na SMTP server
                await smtp.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);

                // autentizace
                await smtp.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);

                // odeslání emailu
                await smtp.SendAsync((MimeMessage)mail);

                // označení jako odeslané
                unsent.Sent = true;

                // uložení změny do DB
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // logování chyby (email zůstane neodeslaný → zkusí se znovu později)
                Console.WriteLine($"Failed to send email with ID {unsent.Id}: {ex.Message}");
            }
        }
    }
}