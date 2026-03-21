using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Reservation.Commands.UpdateReservation;

// Handler → stará se o úpravu existující rezervace (např. změna termínu)
public class UpdateReservationHandler
    : IRequestHandler<UpdateReservationCommand, DetailReservationModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;

    public UpdateReservationHandler(
        AppDbContext appDbContext,
        IClock clock,
        IApplicationMapper applicationMapper)
    {
        _dbContext = appDbContext;
        _clock = clock;
        _mapper = applicationMapper;
    }

    public async Task<DetailReservationModel> Handle(
        UpdateReservationCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. NAČTENÍ REZERVACE
        // =========================================
        var dbEntity = await _dbContext.Reservations
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        // pokud rezervace neexistuje → vracíme null
        if (dbEntity == null)
            return null;

        // =========================================
        // 2. AKTUALIZACE DAT
        // =========================================
        // změna termínu rezervace
        dbEntity.ScheduledAt = request.ScheduledAt;

        // audit (kdo a kdy změnil)
        dbEntity.SetModifyBy("System", _clock.GetCurrentInstant());

        // =========================================
        // 3. ULOŽENÍ ZMĚN
        // =========================================
        await _dbContext.SaveChangesAsync();

        // =========================================
        // 4. ZNOVU NAČTENÍ + MAPOVÁNÍ
        // =========================================
        // znovu načte entitu (např. kvůli vztahům nebo triggerům)
        dbEntity = await _dbContext.Reservations
            .FirstAsync(x => x.Id == dbEntity.Id);

        return _mapper.ToDetail(dbEntity);
    }
}