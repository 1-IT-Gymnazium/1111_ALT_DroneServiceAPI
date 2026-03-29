using DroneService.Application.Contracts.Fields;
using DroneService.Application.Contracts.Utils;
using DroneService.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DroneService.Application.Fields.Queries.GetAllFields;

// Handler → zpracovává GetAllFieldsQuery
// vrací seznam polí s možností filtrování a řazení
public class GetAllFieldsHandler
    : IRequestHandler<GetAllFieldsQuery, List<DetailFieldModel>>
{
    private readonly AppDbContext _dbContext;
    private readonly IApplicationMapper _mapper;

    public GetAllFieldsHandler(AppDbContext dbContext, IApplicationMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<DetailFieldModel>> Handle(
        GetAllFieldsQuery request,
        CancellationToken cancellationToken)
    {
        // =========================================
        // 1. ZÁKLADNÍ QUERY
        // =========================================
        // vytvoří IQueryable → zatím se nic nespouští v DB
        var query = _dbContext.Fields
            .Include(x => x.Author) // načte i autora pole
            .AsQueryable();

        // =========================================
        // 2. FILTRY (postupně skládáš query)
        // =========================================

        // filtr podle názvu (contains = LIKE v SQL)
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(x =>
                x.Name.Contains(request.Name));
        }

        // filtr podle obce
        if (!string.IsNullOrWhiteSpace(request.Municipality))
        {
            query = query.Where(x =>
                x.Municipality.Contains(request.Municipality));
        }

        // filtr podle typu bloku (přesná shoda)
        if (!string.IsNullOrWhiteSpace(request.BlockType))
        {
            query = query.Where(x =>
                x.BlockType == request.BlockType);
        }

        // =========================================
        // 3. ŘAZENÍ
        // =========================================
        if (request.SortBy == "area")
        {
            query = request.SortDirection == "asc"
                ? query.OrderBy(x => x.Area)
                : query.OrderByDescending(x => x.Area);
        }

        // =========================================
        // 4. PROVEDENÍ DOTAZU + MAPOVÁNÍ
        // =========================================
        return await query

            // převod entity → DTO
            .Select(x => _mapper.ToDetailField(x))

            // tady se query skutečně spustí v DB
            .ToListAsync(cancellationToken);
    }
}