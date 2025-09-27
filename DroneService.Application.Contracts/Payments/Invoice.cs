using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Payments;

public class Invoice
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public decimal AmountWithoutVAT { get; set; }
    public decimal VAT { get; set; }
    public decimal AmountWithVAT { get; set; }
    public DateTime CreatedAt { get; set; }
}
