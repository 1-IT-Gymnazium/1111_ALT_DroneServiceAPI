using MediatR;

namespace DroneService.Application.Fields.Commands.DeleteField;

// Command → reprezentuje požadavek na smazání pole (field)
// Vrací bool:
// - true = úspěšně smazáno
// - false = pole neexistovalo
public class DeleteFieldCommand : IRequest<bool>
{
    // ID pole, které chceme smazat
    public Guid Id { get; set; }

    // Konstruktor – nastaví ID při vytvoření commandu
    public DeleteFieldCommand(Guid id)
    {
        Id = id;
    }
}