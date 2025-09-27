using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        // 1. Zavoláme LPIS API
        var fields = await _arcGisService.GetFieldsByLpisIdAsync(request.LpisId);

        if (fields.Count == 0)
            throw new InvalidOperationException($"Žádné pole pro LPIS_ID {request.LpisId} nenalezeno.");

        var dto = fields.First(); // můžeš i vybrat více, záleží na use-case

        // 2. Namapujeme do entity
        var now = _clock.GetCurrentInstant();
        var newEntity = new Field
        {
            Id = Guid.NewGuid(),
            Name = null!, // uživatel doplní později
            CurrentCrops = null!, // uživatel doplní později
            AtticBlock = dto.AtticBlock,
            BlockType = dto.BlockType,
            Municipality = dto.Municipality,
            AuthorId = request.AuthorId
        }.SetCreateBySystem(now);

        // 3. Uložíme
        _dbContext.Fields.Add(newEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 4. Vrátíme detail
        newEntity = await _dbContext
            .Fields
            .Include(x => x.Author)
            .FirstAsync(x => x.Id == newEntity.Id, cancellationToken);

        return _mapper.ToDetailField(newEntity);
    }
}
