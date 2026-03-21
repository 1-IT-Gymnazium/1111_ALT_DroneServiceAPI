using DroneService.Utilities.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using DroneService.Application.Contracts.Interfaces;

namespace DroneService.Application.Shared.BackgroundWorkers;

// Background service, který běží na pozadí aplikace (např. při startu API)
// Slouží pro pravidelné odesílání emailů (např. z fronty v DB)
public class EmailSenderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _provider; // umožňuje vytvářet scope a získávat služby
    private readonly SmtpOptions _smtpOptions;   // konfigurace SMTP (není zde přímo použita, ale může být potřeba v budoucnu)

    public EmailSenderBackgroundService(
        IServiceProvider provider,
        IOptions<SmtpOptions> smtpOptions)
    {
        _provider = provider;
        _smtpOptions = smtpOptions.Value;
    }

    // Hlavní metoda background service
    // Spustí se automaticky při startu aplikace
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SendEmails(stoppingToken);
    }

    // Metoda, která periodicky odesílá emaily
    private async Task SendEmails(CancellationToken stoppingToken)
    {
        // nekonečný loop dokud není aplikace ukončena
        while (!stoppingToken.IsCancellationRequested)
        {
            // vytvoření nového DI scope (důležité pro scoped služby, např. DbContext)
            using var scope = _provider.CreateScope();

            // získání služby pro odesílání emailů
            var emailSenderService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();

            // samotné odeslání emailů (typicky z DB fronty)
            await emailSenderService.SendEmailAsync();

            // čekání 5 minut (300 sekund), než se proces zopakuje
            await Task.Delay(TimeSpan.FromSeconds(300));
        }
    }
}