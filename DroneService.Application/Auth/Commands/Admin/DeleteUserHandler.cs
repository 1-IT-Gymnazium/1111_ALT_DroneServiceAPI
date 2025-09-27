using DroneService.Application.Fields.Commands;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Auth.Commands.Admin;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public DeleteUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var dbEntity = await _dbContext.AppUsers.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (dbEntity == null) return false;

        _dbContext.AppUsers.Remove(dbEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
