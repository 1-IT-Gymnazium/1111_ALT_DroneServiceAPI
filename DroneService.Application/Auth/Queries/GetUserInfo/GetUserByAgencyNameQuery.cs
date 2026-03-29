using DroneService.Application.Contracts.Auth;
using MediatR;

namespace DroneService.Application.Auth.Queries.GetUserInfo;

// Query → požadavek na získání informací o uživateli
// Název napovídá, že se hledá podle AgencyName,
// ale ve skutečnosti se používá UserId (viz handler)
public class GetUserByAgencyNameQuery : IRequest<UserInfo>
{
    // ID uživatele ve string formátu
    // musí se následně parsovat na Guid v handleru
    public string UserId { get; set; }

    // Konstruktor pro vytvoření query
    public GetUserByAgencyNameQuery(string userId)
    {
        UserId = userId;
    }
}