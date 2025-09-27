using DroneService.Application.Contracts.Interfaces;
using DroneService.Application.Contracts.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Services;

public class InvoiceService : IInvoiceService
{
    public Invoice GenerateInvoice(string userId, decimal amount, decimal vat)
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AmountWithoutVAT = amount,
            VAT = vat,
            AmountWithVAT = amount + vat,
            CreatedAt = DateTime.UtcNow
        };

        // Save invoice to DB or generate PDF
        return invoice;
    }
}
