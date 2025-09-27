using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Fields.Commands.Handlers;
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

    public async Task<DetailFieldModel> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        var now = _clock.GetCurrentInstant();
        var newEntity = new Field
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Area = request.Area,
            CurrentCrops = request.CurrentCrops,
            AtticBlock = request.AtticBlock,
            BlockType = request.BlockType,
            Municipality = request.Municipality,
            AuthorId = request.AuthorId,
        }.SetCreateBySystem(now);

        _dbContext.Add(newEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

       newEntity = await _dbContext
           .Fields
           .Include(x => x.Author)
           .FirstAsync(x => x.Id == newEntity.Id, cancellationToken);

        return _mapper.ToDetailField(newEntity);
    }
}
