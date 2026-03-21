using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Reservations;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.Reservation.Queries.GetReservationFiltered;

// Handler pro zpracování filtrovacího dotazu na rezervace
// Tady se z jednoduchého Query objektu stává reálný SQL dotaz nad databází
public class GetReservationsFilteredHandler
    : IRequestHandler<GetReservationsFilteredQuery, List<DetailReservationModel>>
{
    private readonly AppDbContext _dbContext;

    // DbContext se injektuje přes DI → přístup k databázi
    public GetReservationsFilteredHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DetailReservationModel>> Handle(
        GetReservationsFilteredQuery request,
        CancellationToken ct)
    {
        // Začínáme s IQueryable → důležité!
        // Díky tomu se dotaz skládá postupně a přeloží se až na konci do SQL
        var query = _dbContext.Reservations
            .Include(r => r.Fields)   // načtení navázaných polí (JOIN)
            .Include(r => r.Author)   // načtení autora rezervace (JOIN)
            .AsQueryable();

        // Filtr: od kdy (časový rozsah)
        if (request.From is not null)
            query = query.Where(r => r.ScheduledAt >= request.From);

        // Filtr: do kdy
        if (request.To is not null)
            query = query.Where(r => r.ScheduledAt <= request.To);

        // Filtr: stav rezervace (např. Created, Approved...)
        if (request.State.HasValue)
            query = query.Where(r => r.State == request.State.Value);

        // Filtr: typ služby (string → pozor na přesnost)
        if (!string.IsNullOrWhiteSpace(request.ServiceType))
            query = query.Where(r => r.ServiceType == request.ServiceType);

        // Filtr: konkrétní uživatel
        if (request.UserId.HasValue)
            query = query.Where(r => r.AuthorId == request.UserId.Value);

        // Finální část:
        // 1) seřazení
        // 2) projekce (mapování na DTO)
        // 3) spuštění dotazu (ToListAsync)
        return await query
            .OrderByDescending(r => r.CreatedAt) // nejnovější první
            .Select(r => new DetailReservationModel
            {
                Id = r.Id,
                ScheduledAt = r.ScheduledAt,

                // enum → string (lepší pro frontend)
                State = r.State.ToString(),

                Price = r.Price,
                ServiceType = r.ServiceType,

                // ochrana proti null (když by nebyl načten autor)
                AgencyName = r.Author != null ? r.Author.AgencyName : "—",

                // mapování kolekce (Fields → DTO)
                Fields = r.Fields.Select(f => new DetailFieldModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Area = f.Area
                }).ToList()
            })
            .ToListAsync(ct); // tady se query skutečně provede v DB
    }
}