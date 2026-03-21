using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Reservation.Commands.DeleteReservation;

// Handler → stará se o smazání rezervace z databáze
public class DeleteReservationHandler
    : IRequestHandler<DeleteReservationCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public DeleteReservationHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        DeleteReservationCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. NAČTENÍ REZERVACE
        // =========================================
        var dbEntity = await _dbContext.Reservations
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        // pokud rezervace neexistuje → vracíme false
        if (dbEntity == null)
            return false;

        // =========================================
        // 2. SMAZÁNÍ ENTITY
        // =========================================
        _dbContext.Reservations.Remove(dbEntity);

        // =========================================
        // 3. ULOŽENÍ ZMĚN
        // =========================================
        await _dbContext.SaveChangesAsync();

        // =========================================
        // 4. VÝSLEDEK
        // =========================================
        // true = smazání proběhlo
        return true;
    }
}