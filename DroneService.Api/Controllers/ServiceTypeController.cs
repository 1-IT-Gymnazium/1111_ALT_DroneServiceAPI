using DroneService.Application.Contracts.ServiceType;
using DroneService.Application.Contracts.Utils;
using DroneService.Application.ServiceTypes.Command.CreateService;
using DroneService.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DroneService.Api.Controllers;

// Označuje controller jako API controller (automatická validace, binding atd.)
[ApiController]

// URL bude: /api/ServiceType
[Route("api/[controller]")]
public class ServiceTypeController : ControllerBase
{
    // MediatR – používá se pro CQS (Command Query Separation)
    // Controller neposílá logiku přímo do služby, ale přes "command"
    private readonly IMediator _mediator;

    // Dependency Injection – MediatR se sem automaticky injectne
    public ServiceTypeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================================
    // CREATE (vytvoření nové služby)
    // =========================================

    // Endpoint vyžaduje přihlášeného uživatele (JWT)
    [Authorize]

    // POST /api/ServiceType
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceModel model)
    {
        // Vytvoříme command (CQS pattern)
        // Posíláme tam data z requestu + ID aktuálního uživatele z JWT
        var command = new CreateServiceCommand(model, User.GetUserId());

        // Mediator zavolá správný handler (CreateServiceCommandHandler)
        var result = await _mediator.Send(command) as DetailServiceModel;

        // Vrací HTTP 201 + lokaci nového resource
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    // =========================================
    // READ (získání všech služeb)
    // =========================================

    // GET /api/ServiceType
    [HttpGet]
    public IActionResult GetAll([FromServices] AppDbContext db)
    {
        // [FromServices] = DI přímo do parametru metody
        // (nemusíš to mít jako field v controlleru)

        // Načteme všechny service typy z DB
        // a mapujeme je ručně do DTO (DetailServiceModel)
        var services = db.ServiceType.Select(s => new DetailServiceModel
        {
            Id = s.Id,
            Name = s.Name,
            IsSubscription = s.IsSubscription,
        }).ToList();

        return Ok(services);
    }

    // =========================================
    // UPDATE (úprava služby)
    // =========================================

    // PUT /api/ServiceType/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] DetailServiceModel model, [FromServices] AppDbContext db)
    {
        // Najdeme entitu podle ID
        var service = await db.ServiceType.FindAsync(id);

        // Pokud neexistuje → 404
        if (service == null) return NotFound();

        // Přepíšeme hodnoty z requestu
        service.Name = model.Name;
        service.IsSubscription = model.IsSubscription;

        // Uložíme změny do DB
        await db.SaveChangesAsync();

        return Ok(model);
    }

    // =========================================
    // DELETE (smazání služby)
    // =========================================

    // DELETE /api/ServiceType/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromServices] AppDbContext db)
    {
        // Najdeme entitu
        var service = await db.ServiceType.FindAsync(id);

        // Pokud neexistuje → 404
        if (service == null) return NotFound();

        // Odebrání z DB
        db.ServiceType.Remove(service);

        // Uložení změn
        await db.SaveChangesAsync();

        // 204 = úspěch bez obsahu
        return NoContent();
    }
}