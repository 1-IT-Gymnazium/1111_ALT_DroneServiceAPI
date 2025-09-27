using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Reservations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Reservations.Commands;

public class UpdateReservationCommand : IRequest<DetailReservationModel>
{
    public Guid Id { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Location { get; set; } = null!;
    public string ServiceType { get; set; } = null!;

    public UpdateReservationCommand(Guid id, CreateReservationModel model)
    {
        Id = id;
        ScheduledAt = model.ScheduledAt;
        Location = model.Location;
        ServiceType = model.ServiceType;
    }
}
