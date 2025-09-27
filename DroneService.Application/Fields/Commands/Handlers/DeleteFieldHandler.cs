using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Fields.Commands.Handlers;
public class DeleteFieldHandler : IRequestHandler<DeleteFieldCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public DeleteFieldHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
    {
        var dbEntity = await _dbContext.Fields.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (dbEntity == null) return false;

        _dbContext.Fields.Remove(dbEntity);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
