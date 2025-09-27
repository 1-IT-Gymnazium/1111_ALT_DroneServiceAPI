using DroneService.Application.Contracts.Payments;
using DroneService.Data;
using DroneService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DroneService.Application.Contracts.Services;

public class KlarnaService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IClock _clock;
    private readonly AppDbContext _dbContext;

    public KlarnaService(HttpClient httpClient, IConfiguration config, IClock clock, AppDbContext dbContext)
    {
        _httpClient = httpClient;
        _config = config;
        _clock = clock;
        _dbContext = dbContext;
    }

    public async Task<PaymentResult> CreateSubscriptionSessionAsync(PaymentRequest request, decimal price)
    {
        var klarnaUsername = _config["Klarna:Username"];
        var klarnaPassword = _config["Klarna:Password"];
        var baseUrl = _config["Klarna:BaseUrl"];

        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{klarnaUsername}:{klarnaPassword}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        var payload = new
        {
            purchase_country = "CZ",
            purchase_currency = "CZK",
            locale = "cs-CZ",
            order_amount = (int)(price * 100),
            order_tax_amount = 0,
            order_lines = new[]
            {
            new {
                name = request.ServiceType,
                quantity = 1,
                unit_price = (int)(price * 100),
                total_amount = (int)(price * 100),
                total_tax_amount = 0
            }
        },
            merchant_urls = new
            {
                terms = "https://yourdomain.com/terms",
                checkout = "https://yourdomain.com/checkout",
                confirmation = "https://yourdomain.com/confirmation",
                push = "https://yourdomain.com/api/subscription/webhook"
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/payments/v1/sessions", payload);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return new PaymentResult
            {
                Success = false,
                Provider = "Klarna",
                Message = $"Klarna error: {error}",
                PaymentType = "Subscription",
                Amount = price
            };
        }

        var result = await response.Content.ReadFromJsonAsync<KlarnaSessionResponse>();
        return new PaymentResult
        {
            Success = true,
            Provider = "Klarna",
            ClientToken = result.client_token,
            PaymentType = "Subscription",
            Amount = price
        };
    }
    public async Task<PaymentResult> CreateOneTimeSessionAsync(PaymentRequest request, decimal price)
    {
        var klarnaUsername = _config["Klarna:Username"];
        var klarnaPassword = _config["Klarna:Password"];
        var baseUrl = _config["Klarna:BaseUrl"];

        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{klarnaUsername}:{klarnaPassword}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        var payload = new
        {
            purchase_country = "CZ",
            purchase_currency = "CZK",
            locale = "cs-CZ",
            order_amount = (int)(price * 100),
            order_tax_amount = 0,
            order_lines = new[]
            {
            new {
                name = request.ServiceType + " (One-Time)",
                quantity = 1,
                unit_price = (int)(price * 100),
                total_amount = (int)(price * 100),
                total_tax_amount = 0
            }
        },
            merchant_urls = new
            {
                terms = "https://yourdomain.com/terms",
                checkout = "https://yourdomain.com/checkout",
                confirmation = "https://yourdomain.com/confirmation",
                push = "https://yourdomain.com/api/payment/webhook"
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/payments/v1/sessions", payload);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return new PaymentResult
            {
                Success = false,
                Provider = "Klarna",
                Message = $"Klarna error: {error}",
                PaymentType = "OneTime",
                Amount = price
            };
        }

        var result = await response.Content.ReadFromJsonAsync<KlarnaSessionResponse>();
        return new PaymentResult
        {
            Success = true,
            Provider = "Klarna",
            ClientToken = result.client_token,
            PaymentType = "OneTime",
            Amount = price
        };
    }
    public async Task SavePendingSubscription(PaymentRequest request, decimal price)
    {
        var subscription = new DroneService.Data.Entities.Subscription
        {
            AuthorId = request.UserId,
            ServiceType = request.ServiceType,
            Price = (int)price,
            Status = false,
            CreatedAt = _clock.GetCurrentInstant(),
            CreatedBy = request.UserId.ToString(),
            ModifiedAt = _clock.GetCurrentInstant(),
            ModifiedBy = request.UserId.ToString(),
        };

        await _dbContext.Subscriptions.AddAsync(subscription);
        await _dbContext.SaveChangesAsync();
    }
}

public class KlarnaSessionResponse
{
    public string session_id { get; set; } = null!;
    public string client_token { get; set; } = null!;
}

