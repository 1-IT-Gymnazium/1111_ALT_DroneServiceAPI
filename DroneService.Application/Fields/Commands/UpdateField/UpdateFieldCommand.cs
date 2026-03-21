using DroneService.Application.Contracts.Fields;
using MediatR;

namespace DroneService.Application.Fields.Commands.UpdateField;

// Command → reprezentuje požadavek na aktualizaci pole (field)
// Vrací DetailFieldModel (tedy aktualizovaná data)
public class UpdateFieldCommand : IRequest<DetailFieldModel>
{
    // ID pole, které chceme upravit
    public Guid Id { get; set; }

    // Nový název pole
    public string Name { get; set; }

    // Nová informace o plodinách na poli
    public string CurrentCrops { get; set; }

    // Konstruktor – mapuje data z CreateFieldModel na UpdateFieldCommand
    public UpdateFieldCommand(Guid id, CreateFieldModel model)
    {
        Id = id;

        // Převzetí hodnot z modelu (typicky z API requestu)
        Name = model.Name;
        CurrentCrops = model.CurrentCrops;
    }
}