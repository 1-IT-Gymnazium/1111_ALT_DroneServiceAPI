using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Fields.Queries.GetUsersFields;

// Handler → zpracovává GetUserFieldsQuery
// tedy vrací všechna pole konkrétního uživatele
public class GetUserFieldsHandler
    : IRequestHandler<GetUserFieldsQuery, List<DetailFieldModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationMapper _mapper;

    public GetUserFieldsHandler(AppDbContext dbContext, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<DetailFieldModel>> Handle(
        GetUserFieldsQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. DOTAZ DO DB
        // =========================================
        var fields = await _dbContext.Fields

            // načte i navázaného uživatele (Author)
            .Include(f => f.Author)

            // filtr → pouze pole konkrétního uživatele
            .Where(f => f.AuthorId == request.AuthorId)

            // provede SQL dotaz a načte data do paměti
            .ToListAsync(cancellationToken);

        // =========================================
        // 2. MAPOVÁNÍ NA DTO
        // =========================================
        // každou entitu převede na DetailFieldModel
        return fields
            .Select(_mapper.ToDetailField)
            .ToList();
    }
}