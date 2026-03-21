using MediatR;

namespace DroneService.Application.Auth.Commands.Admin;

// Command v CQS patternu → reprezentuje akci "smaž uživatele"
// IRequest<bool> znamená:
// → po vykonání handler vrátí true / false (úspěch / neúspěch)
public class DeleteUserCommand : IRequest<bool>
{
    // ID uživatele, kterého chceme smazat
    public Guid Id { get; set; }

    // Konstruktor → nastaví ID při vytvoření commandu
    public DeleteUserCommand(Guid id)
    {
        Id = id;
    }
}