namespace DroneService.Application.Contracts.ServiceType;

public class CreateServiceModel
{
    public string Name { get; set; } = null!;
    public bool IsSubscription { get; set; }
}
