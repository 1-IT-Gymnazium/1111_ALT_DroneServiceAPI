using DroneService.Utilities.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using DroneService.Application.Contracts.Services;
using DroneService.Data.Interfaces;
using DroneService.Application.Contracts.Interfaces;

namespace DroneService.Application.Shared.BackgroundWorkers;

public class EmailSenderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly SmtpOptions _smtpOptions;

    public EmailSenderBackgroundService(
        IServiceProvider provider,
        IOptions<SmtpOptions> smtpOptions)
    {
        _provider = provider;
        _smtpOptions = smtpOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SendEmails(stoppingToken);
    }

    private async Task SendEmails(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var emailSenderService = scope.ServiceProvider.GetRequiredService<IEmailSenderService>();
            await emailSenderService.SendEmailAsync();
            
            await Task.Delay(TimeSpan.FromSeconds(300));
        }
    }
}
