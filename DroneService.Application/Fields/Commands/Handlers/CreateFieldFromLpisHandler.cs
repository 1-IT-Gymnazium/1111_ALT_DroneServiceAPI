using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Interfaces;
using MediatR;
using NodaTime;

namespace DroneService.Application.Fields.Commands.Handlers;

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

    public async Task<DetailFieldModel> Handle(CreateFieldFromLpisCommand request, CancellationToken cancellationToken)
    {
        // 1. Načti data z ArcGIS API
        Instant now = _clock.GetCurrentInstant();
        var fieldsFromLpis = await _arcGisService.GetFieldsByLpisIdAsync(request.LpisId);

        if (fieldsFromLpis == null || fieldsFromLpis.Count == 0)
        {
            throw new InvalidOperationException($"Žádné pole pro LPIS_ID {request.LpisId} nenalezeno.");
        }

        DetailFieldModel? lastCreated = null;

        // 2. Pro každý FieldDto vytvoř entitu a ulož
        foreach (var dto in fieldsFromLpis)
        {
            var entity = new Field
            {
                Id = Guid.NewGuid(),
                Name = "Nové pole", // uživatel si upraví
                CurrentCrops = string.Empty, // zatím prázdné
                Area = dto.Area,
                AtticBlock = dto.AtticBlock,
                BlockType = dto.BlockType,
                Municipality = dto.Municipality,
                AuthorId = request.AuthorId,
            }.SetCreateBySystem(now); // ⬅️ tohle doplní CreatedAt/By + ModifiedAt/By

            _dbContext.Fields.Add(entity);

            lastCreated = _mapper.ToDetailField(entity); // můžeš použít mapper, ať se držíš konzistence
        }


        // 3. Ulož vše do DB
        await _dbContext.SaveChangesAsync(cancellationToken);

        return lastCreated!;
    }
}
