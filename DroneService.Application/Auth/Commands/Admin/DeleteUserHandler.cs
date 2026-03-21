using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Auth.Commands.Admin;

// Handler pro command DeleteUserCommand
// IRequestHandler<Command, ReturnType>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
{
    // DbContext = přístup k databázi (EF Core)
    private readonly AppDbContext _dbContext;

    // Dependency Injection → DbContext se injectne automaticky
    public DeleteUserHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Hlavní metoda handleru (spustí se při _mediator.Send(command))
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // Pokusíme se najít uživatele podle ID
        var dbEntity = await _dbContext.AppUsers
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        // Pokud uživatel neexistuje → vracíme false
        if (dbEntity == null) return false;

        // Označí entitu pro smazání
        _dbContext.AppUsers.Remove(dbEntity);

        // Uloží změny do databáze (DELETE SQL příkaz)
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Vracíme true → smazání proběhlo
        return true;
    }
}