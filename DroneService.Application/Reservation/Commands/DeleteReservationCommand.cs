using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Reservations.Commands;

public class DeleteReservationCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteReservationCommand(Guid id)
    {
        Id = id;
    }
}
