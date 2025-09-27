using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Reservations.Commands.Handlers;

public class DeleteReservationHandler : IRequestHandler<DeleteReservationCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public DeleteReservationHandler (AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
    {
        var dbEntity = await _dbContext.Reservations.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (dbEntity == null) return false;

        _dbContext.Reservations.Remove(dbEntity);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
