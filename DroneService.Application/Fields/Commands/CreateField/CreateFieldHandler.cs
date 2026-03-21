using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Fields.Commands.CreateField;

// Handler → zpracovává CreateFieldCommand
// tedy vytvoření nového pole v databázi
public class CreateFieldHandler : IRequestHandler<CreateFieldCommand, DetailFieldModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;

    public CreateFieldHandler(AppDbContext dbContext, IClock clock, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<DetailFieldModel> Handle(
        CreateFieldCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. ZÍSKÁNÍ AKTUÁLNÍHO ČASU
        // =========================================
        // Používá se pro audit (CreatedAt apod.)
        var now = _clock.GetCurrentInstant();

        // =========================================
        // 2. VYTVOŘENÍ ENTITY
        // =========================================
        var newEntity = new Field
        {
            // nové ID pro záznam
            Id = Guid.NewGuid(),

            // mapování dat z commandu
            Name = request.Name,
            Area = request.Area,
            CurrentCrops = request.CurrentCrops,
            AtticBlock = request.AtticBlock,
            BlockType = request.BlockType,
            Municipality = request.Municipality,

            // vazba na uživatele (vlastník pole)
            AuthorId = request.AuthorId,

            // nastaví auditní informace (CreatedBy, CreatedAt)
        }.SetCreateBySystem(now);

        // =========================================
        // 3. ULOŽENÍ DO DB
        // =========================================
        _dbContext.Add(newEntity);

        // EF provede INSERT
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========================================
        // 4. ZNOVUNAČTENÍ ENTITY
        // =========================================
        // Načítáme znovu, protože chceme:
        // - includnout Author (navigační property)
        // - mít kompletní data pro mapping
        newEntity = await _dbContext
            .Fields
            .Include(x => x.Author)
            .FirstAsync(x => x.Id == newEntity.Id, cancellationToken);

        // =========================================
        // 5. MAPOVÁNÍ NA DTO
        // =========================================
        return _mapper.ToDetailField(newEntity);
    }
}