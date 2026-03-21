using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Contracts.Result;
using DroneService.Application.Contracts.Services;
using DroneService.Application.Contracts.Utils;
using DroneService.Application.Reservation.Commands.CreateReservation;
using DroneService.Data;
using DroneService.Data.Entities;
using DroneService.Data.Enums;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

// Handler → vytváří novou rezervaci v systému
public class CreateReservationHandler
    : IRequestHandler<CreateReservationCommand, Result<DetailReservationModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly UserContext _userContext;
    private readonly IApplicationMapper _mapper;

    public CreateReservationHandler(
        AppDbContext dbContext,
        IClock clock,
        UserContext userContext,
        IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _clock = clock;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<DetailReservationModel>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. ZÍSKÁNÍ UŽIVATELE
        // =========================================
        var authorId = _userContext.GetUserId();

        // =========================================
        // 2. NAČTENÍ SERVICE TYPE (TYP SLUŽBY)
        // =========================================
        var serviceTypeEntity = await _dbContext.ServiceType
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.AuthorId == authorId, cancellationToken);

        if (serviceTypeEntity == null)
            return Result<DetailReservationModel>
                .Fail("Service type for this user not found");

        // uložíme si důležité informace
        bool isSubscription = serviceTypeEntity.IsSubscription;
        var serviceTypeName = serviceTypeEntity.Name;

        // =========================================
        // 3. NAČTENÍ POLÍ
        // =========================================
        var fields = await _dbContext.Fields
            .Where(f =>
                request.FieldIds.Contains(f.Id) &&
                f.AuthorId == authorId) // bezpečnost → jen vlastní pole
            .ToListAsync(cancellationToken);

        if (fields.Count == 0)
            return Result<DetailReservationModel>
                .Fail("No valid fields found.");

        // =========================================
        // 4. VÝPOČET CELKOVÉ ROZLOHY
        // =========================================
        var totalArea = fields.Sum(f => f.Area);

        // =========================================
        // 5. VÝPOČET CENY ZA HEKTAR
        // =========================================
        decimal pricePerHectare = isSubscription
            ? serviceTypeName switch
            {
                "Basic" => 40m,
                "Premium" => 120m,
                "Enterprise" => 400m,
                _ => 0m
            }
            : serviceTypeName switch
            {
                "Scan" => 250m,
                "Scan i aplikace" => 800m,
                "Aplikace" => 750m,
                _ => 0m
            };

        // ochrana → neznámý typ služby
        if (pricePerHectare == 0)
            return Result<DetailReservationModel>
                .Fail("Invalid service type.");

        // =========================================
        // 6. VÝPOČET CELKOVÉ CENY
        // =========================================
        decimal totalPrice = pricePerHectare * (decimal)totalArea;

        // =========================================
        // 7. VYTVOŘENÍ ENTITY
        // =========================================
        var now = _clock.GetCurrentInstant();

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            ScheduledAt = request.ScheduledAt,
            ServiceType = serviceTypeName,
            State = ReservationState.Created,
            Price = totalPrice,
            IsSubscription = isSubscription,
            Fields = fields, // vazba na pole
        }
        .SetCreateBySystem(now);

        // =========================================
        // 8. ULOŽENÍ DO DB
        // =========================================
        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========================================
        // 9. NAČTENÍ NAVIGAČNÍCH DAT
        // =========================================
        await _dbContext.Entry(reservation)
            .Collection(r => r.Fields)
            .LoadAsync(cancellationToken);

        // =========================================
        // 10. MAPOVÁNÍ NA DTO
        // =========================================
        var dto = _mapper.ToDetail(reservation);

        return Result<DetailReservationModel>.Ok(dto);
    }
}