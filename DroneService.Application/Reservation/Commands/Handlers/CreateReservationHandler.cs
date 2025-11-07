using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Contracts.Services;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Reservations.Commands.Handlers;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, DetailReservationModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly UserContext _userContext;

    public CreateReservationHandler(AppDbContext dbContext, IClock clock, UserContext userContext)
    {
        _dbContext = dbContext;
        _clock = clock;
        _userContext = userContext;
    }

    public async Task<DetailReservationModel> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var authorId = _userContext.GetUserId();

        // 1️⃣ Najdi ServiceType, který patří uživateli
        var serviceTypeEntity = await _dbContext.ServiceType
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.AuthorId == authorId, cancellationToken)
            ?? throw new Exception("Service type for this user not found");

        bool isSubscription = serviceTypeEntity.IsSubscription;
        var serviceTypeName = serviceTypeEntity.Name;

        // 2️⃣ Načti pole podle ID
        var fields = await _dbContext.Fields
            .Where(f => request.FieldIds.Contains(f.Id))
            .ToListAsync(cancellationToken);

        if (fields.Count == 0)
            throw new Exception("No valid fields found.");

        // 3️⃣ Spočítej celkovou rozlohu
        var totalArea = fields.Sum(f => f.Area);

        // 4️⃣ Urči cenu podle typu služby
        decimal pricePerHectare = isSubscription
            ? serviceTypeName switch
            {
                "Basic" => 40m,
                "Premium" => 120m,
                "Enterprise" => 400m,
                _ => throw new Exception("Invalid service type.")
            }
            : serviceTypeName switch
            {
                "Scan" => 250m,
                "Scan i aplikace" => 800m,
                "Aplikace" => 750m,
                _ => throw new Exception("Invalid service type.")
            };

        // 5️⃣ Spočítej celkovou cenu
        decimal totalPrice = pricePerHectare * (decimal)totalArea;

        var now = _clock.GetCurrentInstant();

        // 6️⃣ Vytvoř rezervaci
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            ScheduledAt = request.ScheduledAt,
            ServiceType = serviceTypeName,
            Price = totalPrice,
            IsSubscription = isSubscription,
            Fields = fields,
        }.SetCreateBySystem(now);

        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 7️⃣ Vrátíme model
        return new DetailReservationModel
        {
            Id = reservation.Id,
            ScheduledAt = reservation.ScheduledAt,
            ServiceType = reservation.ServiceType,
            Price = reservation.Price,
            IsSubscription = reservation.IsSubscription
        };
    }
}
