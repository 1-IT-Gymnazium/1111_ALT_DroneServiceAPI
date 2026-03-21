using DroneService.Application.Contracts.Reservations;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Reservation.Queries.GetUserReservation;

// Handler → vrací všechny rezervace konkrétního uživatele
public class GetUserReservationQueryHandler
    : IRequestHandler<GetUserReservationQuery, List<DetailReservationModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationMapper _mapper;

    public GetUserReservationQueryHandler(
        AppDbContext dbContext,
        IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<DetailReservationModel>> Handle(
        GetUserReservationQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. NAČTENÍ REZERVACÍ Z DB
        // =========================================
        var reservations = await _dbContext.Reservations
            .Include(r => r.Fields) // načtení souvisejících polí (relation)
            .Where(r => r.AuthorId == request.AuthorId) // jen rezervace konkrétního uživatele
            .ToListAsync(cancellationToken);

        // =========================================
        // 2. MAPOVÁNÍ NA DTO
        // =========================================
        return reservations
            .Select(_mapper.ToDetail)
            .ToList();
    }
}