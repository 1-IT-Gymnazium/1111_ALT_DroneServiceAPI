using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Fields.Commands.DeleteField;

// Handler → zpracovává DeleteFieldCommand
// tedy logiku pro smazání pole z databáze
public class DeleteFieldHandler : IRequestHandler<DeleteFieldCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public DeleteFieldHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        DeleteFieldCommand request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. NAČTENÍ ENTITY Z DB
        // =========================================
        var dbEntity = await _dbContext.Fields
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        // Pokud pole neexistuje → není co mazat
        if (dbEntity == null) return false;

        // =========================================
        // 2. ODSTRANĚNÍ ENTITY
        // =========================================
        // Označí entitu jako "Deleted" v EF
        _dbContext.Fields.Remove(dbEntity);

        // =========================================
        // 3. ULOŽENÍ ZMĚN
        // =========================================
        // EF provede DELETE v databázi
        await _dbContext.SaveChangesAsync();

        // =========================================
        // 4. RETURN
        // =========================================
        return true;
    }
}