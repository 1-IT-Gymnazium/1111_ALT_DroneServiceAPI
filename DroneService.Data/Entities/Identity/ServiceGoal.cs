using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Data.Entities.Identity;

public class ServiceGoal
{
    public Guid Id { get; set; }
    public string Goal { get; set; } = null!;
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}

