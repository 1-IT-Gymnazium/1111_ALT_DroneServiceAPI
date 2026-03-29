using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Fields.Commands.CreateFieldFromLpis;

// Handler → vytvoří pole na základě dat z LPIS (ArcGIS)
// typicky: jeden request → může vytvořit více polí
public class CreateFieldFromLpisHandler : IRequestHandler<CreateFieldFromLpisCommand, DetailFieldModel>
{
    private readonly AppDbContext _dbContext;
    private readonly ArcGisService _arcGisService;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;

    public CreateFieldFromLpisHandler(
        AppDbContext dbContext,
        ArcGisService arcGisService,
        IClock clock,
        IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _arcGisService = arcGisService;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<DetailFieldModel> Handle(
        CreateFieldFromLpisCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. AKTUÁLNÍ ČAS (audit)
        // =========================================
        Instant now = _clock.GetCurrentInstant();

        // =========================================
        // 2. ZÍSKÁNÍ DAT Z EXTERNÍHO API (ArcGIS)
        // =========================================
        var fieldsFromLpis = await _arcGisService
            .GetFieldsByLpisIdAsync(request.LpisId);

        // Pokud nic nepřišlo → fail
        if (fieldsFromLpis == null || fieldsFromLpis.Count == 0)
        {
        // Spadne to tady 
            throw new InvalidOperationException(
                $"Žádné pole pro LPIS_ID {request.LpisId} nenalezeno.");
        }

        // =========================================
        // 3. ZJIŠTĚNÍ POČTU EXISTUJÍCÍCH POLÍ
        // =========================================
        // používá se pro generování názvu "Pole X"
        int existingCount = await _dbContext.Fields
            .CountAsync(f => f.AuthorId == request.AuthorId, cancellationToken);

        DetailFieldModel? lastCreated = null;

        int index = 1;

        // =========================================
        // 4. VYTVOŘENÍ VÍCE ENTIT
        // =========================================
        foreach (var dto in fieldsFromLpis)
        {
            var entity = new Field
            {
                Id = Guid.NewGuid(),

                // generovaný název (pole 1,2,3...)
                Name = $"Pole {existingCount + index}",

                // default hodnoty
                // to může měnit uživatel v dalších krocích 
                CurrentCrops = string.Empty,

                // data z LPIS
                Area = dto.Area,
                AtticBlock = dto.AtticBlock,
                BlockType = dto.BlockType,
                LpisId = dto.LpisId,
                FID = dto.FID,
                dDpb = dto.dDpb,
                Municipality = dto.Municipality,

                // vlastník - uživatel
                AuthorId = request.AuthorId,

                // audit
            }.SetCreateBySystem(now);

            // přidání do EF contextu (zatím jen v paměti)
            _dbContext.Fields.Add(entity);

            // mapujeme poslední vytvořený záznam
            lastCreated = _mapper.ToDetailField(entity);

            index++;
        }

        // =========================================
        // 5. ULOŽENÍ DO DB (batch insert)
        // =========================================
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========================================
        // 6. RETURN
        // =========================================
        // vrací pouze poslední vytvořené pole
        return lastCreated!;
    }
}