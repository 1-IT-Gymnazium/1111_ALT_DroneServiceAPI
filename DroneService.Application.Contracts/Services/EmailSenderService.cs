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

public class EmailSenderService : IEmailSenderService
{
    private readonly AppDbContext _dbContext;
    private readonly SmtpOptions _smtpOptions;
    private readonly EnvironmentOptions _envOptions;
    private readonly IClock _clock;

    public EmailSenderService(IClock clock, AppDbContext appDbContext, IOptions<EnvironmentOptions> envOptions, IOptions<SmtpOptions> options)
    {
        _dbContext = appDbContext;
        _smtpOptions = options.Value;
        _envOptions = envOptions.Value;
        _clock = clock;
    }

    public async Task AddEmail(string subject, string body, string recipientEmail, string? recipientName = null, string? fromEmail = null, string? fromName = null)
    {
        var fromEmailValue = fromEmail ?? _envOptions.SenderEmail;
        var fromNameValue = fromName ?? _envOptions.SenderName;

        var message = new EmailMessage
        {
            Subject = subject,
            Body = body,
            RecipientEmail = recipientEmail,
            RecipientName = recipientName,
            FromEmail = fromEmailValue,
            FromName = fromNameValue,
            Sent = false,
            CreatedAt = _clock.GetCurrentInstant(),
        };
        _dbContext.Add(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SendEmailAsync()
    {
        var unsentMails = await _dbContext.Emails.Where(x => !x.Sent).ToListAsync();
        foreach (var unsent in unsentMails)
        {
            if (string.IsNullOrEmpty(unsent.FromEmail))
            {
                Console.WriteLine($"Skipping email with ID {unsent.Id}: FromEmail is null or empty.");
                continue;
            }

            if (string.IsNullOrEmpty(unsent.RecipientEmail))
            {
                Console.WriteLine($"Skipping email with ID {unsent.Id}: RecipientEmail is null or empty.");
                continue;
            }


            var fromName = string.IsNullOrEmpty(unsent.FromName) ? _envOptions.SenderName : unsent.FromName;
            var recipientName = string.IsNullOrEmpty(unsent.RecipientName) ? "Recipient" : unsent.RecipientName;

            using var mail = new MailMessage
            {
                Subject = unsent.Subject,
                Body = unsent.Body,
                IsBodyHtml = true,
                From = new MailAddress(unsent.FromEmail, fromName),
            };
            mail.To.Add(new MailAddress(unsent.RecipientEmail, recipientName));

            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port);
                await smtp.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password);
                await smtp.SendAsync((MimeMessage)mail);

                unsent.Sent = true;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email with ID {unsent.Id}: {ex.Message}");
            }
        }
    }
}
