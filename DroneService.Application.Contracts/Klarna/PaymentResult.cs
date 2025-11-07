using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Klarna
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Provider { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public string? ClientToken { get; set; }
        public string PaymentType { get; set; }
        public string? RawResponse { get; set; }
    }
}
