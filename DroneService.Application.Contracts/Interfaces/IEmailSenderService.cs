namespace DroneService.Application.Contracts.Interfaces;

// Interface pro práci s emaily
// Odděluje logiku přidávání emailů (např. do fronty) a jejich odesílání
public interface IEmailSenderService
{
    // Přidá email do systému (typicky do DB/fronty)
    // Email se neodesílá hned, ale zpracuje ho background service
    Task AddEmail(
        string subject,           // předmět emailu
        string body,              // obsah emailu (HTML / text)
        string recipientEmail,    // email příjemce
        string? recipientName = null, // jméno příjemce (volitelné)
        string? fromEmail = null,     // email odesílatele (volitelné)
        string? fromName = null       // jméno odesílatele (volitelné)
    );

    // Odeslání emailů (např. všech čekajících ve frontě)
    // Tuto metodu typicky volá BackgroundService
    Task SendEmailAsync();
}