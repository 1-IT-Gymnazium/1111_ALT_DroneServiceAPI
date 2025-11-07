using DroneService.Application.Contracts.ServiceType;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using DroneService.Data.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace DroneService.Application.ServiceTypes.Command.Handler;

public class CreateServiceHandler : IRequestHandler<CreateServiceCommand, DetailServiceModel>
{
    private readonly AppDbContext _dbContext;
    private readonly IClock _clock;
    private readonly IApplicationMapper _mapper;

    public CreateServiceHandler(AppDbContext dbContext, IClock clock, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _clock = clock;
        _mapper = mapper;
    }

    public async Task<DetailServiceModel> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var now = _clock.GetCurrentInstant();
        var service = new DroneService.Data.Entities.ServiceType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsSubscription = request.IsSubscription,
            AuthorId = request.AuthorId,
        }.SetCreateBySystem(now);

        _dbContext.Add(service);
        await _dbContext.SaveChangesAsync(cancellationToken);

        service = await _dbContext
            .ServiceType
            .Include(x => x.Author)
            .FirstAsync(x => x.Id == service.Id, cancellationToken);

        return _mapper.ToDetailField(service);
    }
}
