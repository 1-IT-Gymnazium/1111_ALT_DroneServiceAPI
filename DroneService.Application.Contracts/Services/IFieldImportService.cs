using DroneService.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Services;

public interface IFieldImportService
{
    Task ImportFieldsFromLpisAsync(AppUser user, string arcGisId);
}
