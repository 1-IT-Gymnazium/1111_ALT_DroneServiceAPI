using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Fields.Queries.Handlers;

public class GetUserFieldsHandler : IRequestHandler<GetUserFieldsQuery, List<DetailFieldModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationMapper _mapper;

    public GetUserFieldsHandler(AppDbContext dbContext, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<DetailFieldModel>> Handle(GetUserFieldsQuery request, CancellationToken cancellationToken)
    {
        var fields = await _dbContext.Fields
            .Include(f => f.Author)
            .Where(f => f.AuthorId == request.AuthorId)
            .ToListAsync(cancellationToken);

        return fields.Select(_mapper.ToDetailField).ToList();
    }
}
