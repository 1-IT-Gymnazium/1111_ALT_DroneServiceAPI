using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Reservations.Commands.Handlers;

public class UpdateReservationHandler : IRequestHandler<UpdateReservationCommand, DetailReservationModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;

    public UpdateReservationHandler (AppDbContext appDbContext, IClock clock, IApplicationMapper applicationMapper)
    {
        _dbContext = appDbContext;
        _clock = clock;
        _mapper = applicationMapper;
    }

    public async Task<DetailReservationModel> Handle(UpdateReservationCommand request, CancellationToken cancellationToken)
    {
        var dbEntity = await _dbContext.Reservations.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (dbEntity == null) return null;

        dbEntity.ScheduledAt = request.ScheduledAt;
        dbEntity.ServiceType = request.ServiceType;
        dbEntity.SetModifyBy("System", _clock.GetCurrentInstant());
        await _dbContext.SaveChangesAsync();

        dbEntity = await _dbContext.Reservations.FirstAsync(x => x.Id == dbEntity.Id);
        return _mapper.ToDetail(dbEntity);
    }
}
