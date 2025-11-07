using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Interfaces;

public interface IUserContext
{
    Guid GetUserId();
}
