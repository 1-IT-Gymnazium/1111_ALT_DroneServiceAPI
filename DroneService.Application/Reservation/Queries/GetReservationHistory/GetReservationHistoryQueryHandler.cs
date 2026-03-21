using DroneService.Application.Contracts.Reservations;
using DroneService.Data;
using MediatR;
using NodaTime;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Reservation.Queries.GetReservationHistory;

// Handler pro získání historických rezervací (tj. těch, které už proběhly)
public class GetReservationHistoryQueryHandler
    : IRequestHandler<GetReservationHistoryQuery, List<ReservationDto>>
{
    private readonly AppDbContext _dbContext;

    // DbContext → přístup k databázi
    public GetReservationHistoryQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ReservationDto>> Handle(
        GetReservationHistoryQuery request,
        CancellationToken cancellationToken)
    {
        // Aktuální čas (Instant z NodaTime → přesnější než DateTime)
        var now = SystemClock.Instance.GetCurrentInstant();

        // Základ dotazu:
        // - bereme rezervace
        // - které nejsou smazané (soft delete)
        // - které už proběhly (ScheduledAt < now)
        var query = _dbContext.Reservations
            .AsNoTracking() // výkon: EF nebude sledovat změny (jen čtení)
            .Where(r =>
                r.DeletedAt == null &&
                r.ScheduledAt < now
            );

        // Pokud je zadán konkrétní uživatel → filtrujeme jen jeho historii
        // (používá se např. pro "moje historie")
        if (request.AuthorId.HasValue)
        {
            query = query.Where(r => r.AuthorId == request.AuthorId.Value);
        }

        // Finální část:
        // - seřazení (nejnovější historie nahoře)
        // - mapování na DTO
        // - vykonání dotazu
        return await query
            .OrderByDescending(r => r.ScheduledAt)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                ScheduledAt = r.ScheduledAt,
                ServiceType = r.ServiceType,
                Price = r.Price,
                IsSubscription = r.IsSubscription,

                // enum → string (lepší pro frontend)
                State = r.State.ToString(),

                // pozor: tady se sahá na navigační vlastnost (Author)
                AuthorName = r.Author.DisplayName,

                // mapování kolekce polí na seznam názvů
                FieldNames = r.Fields
                    .Select(f => f.Name)
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }
}