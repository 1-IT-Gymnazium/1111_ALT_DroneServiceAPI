using DroneService.Application.Contracts.Payments;

namespace DroneService.Application.Contracts.Helpers;

public static class PaymentHelpers
{
    public static decimal CalculatePrice(string serviceType, int hectares = 0)
    {
        var basePrice = serviceType switch
        {
            "Basic" => 40,
            "Premium" => 120,
            "Enterprise" => 400,
            _ => throw new Exception("Unknown service type")
        };

        // Example: add price based on area
        var additionalCost = hectares switch
        {
            <= 5 => 0,
            <= 10 => 20,
            _ => 50
        };

        return basePrice + additionalCost;
    }

}

