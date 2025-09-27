using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Fields.Queries.Handlers;

public class GetAllFieldsHandler : IRequestHandler<GetAllFieldsQuery, List<DetailFieldModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationMapper _mapper;

    public GetAllFieldsHandler(AppDbContext dbContext, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<DetailFieldModel>> Handle(GetAllFieldsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.Fields.Include(x => x.Author).Select(x => _mapper.ToDetailField(x)).ToListAsync();
    }
}

