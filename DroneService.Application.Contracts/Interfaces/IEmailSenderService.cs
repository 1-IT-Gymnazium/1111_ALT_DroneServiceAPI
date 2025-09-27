using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Interfaces;

public interface IEmailSenderService
{
    Task AddEmail(string subject, string body, string recipientEmail, string? recipientName = null, string? fromEmail = null, string? fromName = null);
    Task SendEmailAsync();
}

