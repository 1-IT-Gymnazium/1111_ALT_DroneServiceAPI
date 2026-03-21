using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Fields.Commands.UpdateField;

// Handler → zpracovává UpdateFieldCommand
// tedy logiku pro úpravu existujícího pole v databázi
public class UpdateFieldHandler : IRequestHandler<UpdateFieldCommand, DetailFieldModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;

    public UpdateFieldHandler(AppDbContext dbContext, IClock clock, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<DetailFieldModel> Handle(
        UpdateFieldCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. NAČTENÍ ENTITY Z DB
        // =========================================
        var dbEntity = await _dbContext.Fields
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        // Pokud pole neexistuje → vracíme null
        if (dbEntity == null) return null;

        // =========================================
        // 2. AKTUALIZACE DAT
        // =========================================
        dbEntity.Name = request.Name;
        dbEntity.CurrentCrops = request.CurrentCrops;

        // Nastavení metadata (kdo a kdy upravil)
        dbEntity.SetModifyBy("System", _clock.GetCurrentInstant());

        // =========================================
        // 3. ULOŽENÍ DO DB
        // =========================================
        await _dbContext.SaveChangesAsync();

        // =========================================
        // 4. ZNOVUNAČTENÍ ENTITY
        // =========================================
        // Načteme znovu aktuální stav z DB
        dbEntity = await _dbContext.Fields
            .FirstAsync(x => x.Id == dbEntity.Id);

        // =========================================
        // 5. MAPOVÁNÍ NA DTO
        // =========================================
        return _mapper.ToDetailField(dbEntity);
    }
}