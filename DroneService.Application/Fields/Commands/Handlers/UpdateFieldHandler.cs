using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Fields.Commands.Handlers;
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

    public async Task<DetailFieldModel> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
    {
        var dbEntity = await _dbContext.Fields.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (dbEntity == null) return null;

        dbEntity.Name = request.Name;
        dbEntity.CurrentCrops = request.CurrentCrops;
        dbEntity.SetModifyBy("System", _clock.GetCurrentInstant());

        await _dbContext.SaveChangesAsync();

        dbEntity = await _dbContext.Fields.FirstAsync(x => x.Id == dbEntity.Id);
        return _mapper.ToDetailField(dbEntity);
    }
}
