using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Reservation.Queries.GetReservationByDateRange;

// Handler pro získání rezervací v určitém časovém rozsahu
// Typicky používaný např. pro kalendář (např. "rezervace na příštích X dní")
public class GetReservationByDateRangeHandler
    : IRequestHandler<GetReservationsByDateRangeQuery, List<DetailReservationModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationMapper _mapper;

    public GetReservationByDateRangeHandler(
        AppDbContext dbContext,
        IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<DetailReservationModel>> Handle(
        GetReservationsByDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        // Vypočítáme horní hranici intervalu:
        // od "From" přičteme počet dní
        var to = request.From.Plus(Duration.FromDays(request.Days));

        // Dotaz do databáze:
        var reservations = await _dbContext.Reservations
            .AsNoTracking() // jen čtení → lepší výkon

            // načtení navázaných dat (autor + pole)
            .Include(r => r.Author)
            .Include(r => r.Fields)

            // filtr podle intervalu <From, To)
            // (včetně From, bez To → správný interval bez překryvu)
            .Where(r =>
                r.ScheduledAt >= request.From &&
                r.ScheduledAt < to
            )

            // seřazení podle času (od nejbližšího)
            .OrderBy(r => r.ScheduledAt)

            // vykonání dotazu
            .ToListAsync(cancellationToken);

        // Mapování entity → DTO pomocí mapperu
        return reservations
            .Select(r => _mapper.ToDetail(r))
            .ToList();
    }
}