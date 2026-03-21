using DroneService.Application.Contracts.ServiceType;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.ServiceTypes.Command.CreateService;

// Handler pro zpracování CreateServiceCommand
// Obsahuje business logiku pro vytvoření nového ServiceType
public class CreateServiceHandler : IRequestHandler<CreateServiceCommand, DetailServiceModel>
{
    private readonly AppDbContext _dbContext;   // přístup do databáze
    private readonly IClock _clock;             // práce s časem (NodaTime)
    private readonly IApplicationMapper _mapper; // mapování entity → DTO

    public CreateServiceHandler(AppDbContext dbContext, IClock clock, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<DetailServiceModel> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        // získání aktuálního času
        var now = _clock.GetCurrentInstant();

        // vytvoření nové entity ServiceType
        var service = new Data.Entities.ServiceType
        {
            Id = Guid.NewGuid(),                  // generování nového ID
            Name = request.Name,                 // název služby
            IsSubscription = request.IsSubscription, // typ služby (subscription / jednorázová)
            AuthorId = request.AuthorId,         // autor služby
        }.SetCreateBySystem(now); // nastavení auditních údajů (CreatedAt, CreatedBy)

        // přidání do databáze
        _dbContext.Add(service);

        // uložení změn
        await _dbContext.SaveChangesAsync(cancellationToken);

        // znovunačtení entity včetně autora (kvůli mapování)
        service = await _dbContext
            .ServiceType
            .Include(x => x.Author)
            .FirstAsync(x => x.Id == service.Id, cancellationToken);

        // mapování entity na DTO a vrácení výsledku
        return _mapper.ToDetailField(service);
    }
}