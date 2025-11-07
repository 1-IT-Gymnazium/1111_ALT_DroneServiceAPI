using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Interfaces;
using MediatR;
using NodaTime;
using Microsoft.EntityFrameworkCore;

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
        Instant now = _clock.GetCurrentInstant();
        var fieldsFromLpis = await _arcGisService.GetFieldsByLpisIdAsync(request.LpisId);

        if (fieldsFromLpis == null || fieldsFromLpis.Count == 0)
        {
            throw new InvalidOperationException($"Žádné pole pro LPIS_ID {request.LpisId} nenalezeno.");
        }

        int existingCount = await _dbContext.Fields
            .CountAsync(f => f.AuthorId == request.AuthorId, cancellationToken);

        DetailFieldModel? lastCreated = null;

        int index = 1;
        foreach (var dto in fieldsFromLpis)
        {
            var entity = new Field
            {
                Id = Guid.NewGuid(),
                Name = $"Pole {existingCount + index}",
                CurrentCrops = string.Empty,
                Area = dto.Area,
                AtticBlock = dto.AtticBlock,
                BlockType = dto.BlockType,
                Municipality = dto.Municipality,
                AuthorId = request.AuthorId,
            }.SetCreateBySystem(now);

            _dbContext.Fields.Add(entity);

            lastCreated = _mapper.ToDetailField(entity);
            index++;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return lastCreated!;
    }
}
