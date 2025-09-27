using DroneService.Application.Contracts.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Interfaces;

public interface IInvoiceService
{
    Invoice GenerateInvoice(string userId, decimal amount, decimal vat);
}
