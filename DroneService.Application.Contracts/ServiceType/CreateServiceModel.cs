using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.ServiceType;

public class CreateServiceModel
{
    public string Name { get; set; } = null!;
    public bool IsSubscription { get; set; }
}
