using DroneService.Application.Contracts.Fields;
using DroneService.Data.Entities.Identity;
using MediatR;

namespace DroneService.Application.Fields.Commands.CreateField;

// Command → vytvoření nového pole (field)
// Obsahuje všechna data potřebná pro uložení do databáze
public class CreateFieldCommand : IRequest<DetailFieldModel>
{
    // Název pole
    public string Name { get; set; }

    // Velikost pole (např. v hektarech)
    public double Area { get; set; }

    // Informace o aktuálních plodinách
    public string CurrentCrops { get; set; }

    // Identifikace bloku (pravděpodobně z LPIS / katastru)
    public string AtticBlock { get; set; }

    // Typ bloku (např. orná půda, louka…)
    public string BlockType { get; set; } = null!;

    // Obec, kde se pole nachází
    public string Municipality { get; set; } = null!;

    // Celý objekt uživatele (není zde využitý v konstruktoru)
    // pravděpodobně zbytečný nebo určený pro budoucí použití
    public AppUser Author { get; set; }

    // ID uživatele, který pole vytváří
    public Guid AuthorId { get; set; }

    // Konstruktor – mapuje data z CreateFieldModel (API) do commandu
    public CreateFieldCommand(CreateFieldModel model, Guid authorId)
    {
        Name = model.Name;
        Area = model.Area;
        CurrentCrops = model.CurrentCrops;

        // ID autora se bere zvlášť (z JWT)
        AuthorId = authorId;

        AtticBlock = model.AtticBlock;
        BlockType = model.BlockType;
        Municipality = model.Municipality;
    }
}