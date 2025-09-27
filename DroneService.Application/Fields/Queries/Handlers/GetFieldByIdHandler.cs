using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Fields.Queries.Handlers;

public class GetFieldByIdHandler : IRequestHandler<GetFieldByIdQuery, DetailFieldModel?>
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationMapper _mapper;

    public GetFieldByIdHandler(AppDbContext dbContext, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DetailFieldModel?> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        var dbEntity = await _dbContext.Fields.FirstOrDefaultAsync(f => f.Id == request.Id);
        return dbEntity != null ? _mapper.ToDetailField(dbEntity) : null;
    }
}

