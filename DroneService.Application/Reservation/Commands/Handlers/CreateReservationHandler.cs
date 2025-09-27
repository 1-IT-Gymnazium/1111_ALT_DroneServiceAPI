using DroneService.Data.Entities;
using DroneService.Data;
using MediatR;
using NodaTime;
using DroneService.Data.Interfaces;

namespace DroneService.Application.Reservations.Commands.Handlers;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, Guid>
{
    private readonly AppDbContext _context;
    private readonly IClock _clock;

    public CreateReservationHandler(AppDbContext context, IClock clock)
    {
        _context = context;
        _clock = clock;
    }

    public async Task<Guid> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var now = _clock.GetCurrentInstant();

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            AuthorId = request.UserId, 
            ScheduledAt = request.Date,
            Location = request.Location,
            ServiceType = request.ServiceType,
            Note = request.Note,
        }.SetCreateBySystem(now);

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
