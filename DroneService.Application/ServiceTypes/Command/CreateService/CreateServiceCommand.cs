using DroneService.Application.Contracts.ServiceType;
using MediatR;

namespace DroneService.Application.ServiceTypes.Command.CreateService;

// Tento Command slouží k vytvoření nového typu služby (ServiceType)
// V rámci CQS/MediatR reprezentuje "write" operaci (zápis do DB)
public class CreateServiceCommand : IRequest<DetailServiceModel>
// IRequest<DetailServiceModel> znamená,
// že handler po zpracování vrátí detail vytvořené služby
{
    // Název služby (např. "Basic", "Premium", "Scan"...)
    public string Name { get; set; }

    // Určuje, zda se jedná o předplatné (subscription) nebo jednorázovou službu
    public bool IsSubscription { get; set; }

    // ID uživatele, který službu vytváří (autor)
    public Guid AuthorId { get; set; }

    // Konstruktor mapuje data z API modelu (CreateServiceModel)
    // + přidává AuthorId (získané např. z JWT tokenu)
    public CreateServiceCommand(CreateServiceModel model, Guid authorId)
    {
        Name = model.Name;                     // přenesení názvu služby
        IsSubscription = model.IsSubscription; // přenesení typu služby (subscription / one-time)
        AuthorId = authorId;                  // přiřazení autora
    }
}
