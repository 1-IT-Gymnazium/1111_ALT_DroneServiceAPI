using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Reservation.Commands.UpdateReservationStatus;

// Handler → zpracovává command pro změnu stavu rezervace
public class UpdateReservationStatusHandler
    : IRequestHandler<UpdateReservationStatusCommand, DetailReservationModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;

    public UpdateReservationStatusHandler(
        AppDbContext dbContext,
        IClock clock,
        IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<DetailReservationModel> Handle(
        UpdateReservationStatusCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. NAČTENÍ REZERVACE Z DB
        // =========================================
        var reservation = await _dbContext.Reservations
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        // pokud rezervace neexistuje → vracíme null
        if (reservation == null)
            return null;

        // =========================================
        // 2. ZMĚNA STAVU
        // =========================================
        // nastavíme nový status (např. Approved, Rejected...)
        reservation.State = request.Status;

        // audit → kdo a kdy změnu provedl
        reservation.SetModifyBy("Admin", _clock.GetCurrentInstant());

        // =========================================
        // 3. ULOŽENÍ ZMĚN
        // =========================================
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========================================
        // 4. MAPOVÁNÍ NA DTO
        // =========================================
        return _mapper.ToDetail(reservation);
    }
}