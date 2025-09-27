using DroneService.Application.Contracts.Auth;
using DroneService.Application.Contracts.Utils;
using DroneService.Data.Entities.Identity;
using MediatR;

namespace DroneService.Application.Auth.Commands.Register;

public class FormCommand : IRequest<DetailUserModel>
{
    public string AgencyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string ArcGisId { get; set; } = string.Empty;
    public string AgencyAddress { get; set; } = string.Empty;
    public string Ico { get; set; } = string.Empty;
    public List<string> ServiceGoals { get; set; } = new();
}