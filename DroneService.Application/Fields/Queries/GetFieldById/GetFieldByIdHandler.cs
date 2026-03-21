using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Application.Fields.Queries.GetUsersFields;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Fields.Queries.GetFieldById;

// Handler → zpracovává GetFieldByIdQuery
// tedy vrací jedno konkrétní pole podle ID
public class GetFieldByIdHandler
    : IRequestHandler<GetFieldByIdQuery, DetailFieldModel?>
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationMapper _mapper;

    public GetFieldByIdHandler(AppDbContext dbContext, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<DetailFieldModel?> Handle(
        GetFieldByIdQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. NAČTENÍ ENTITY Z DB
        // =========================================
        var dbEntity = await _dbContext.Fields
            .FirstOrDefaultAsync(f => f.Id == request.Id);

        // =========================================
        // 2. MAPOVÁNÍ / RETURN
        // =========================================
        // pokud existuje → mapuj na DTO
        // pokud ne → vrať null
        return dbEntity != null
            ? _mapper.ToDetailField(dbEntity)
            : null;
    }
}