using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Companies.Models;

namespace SalesAI.Application.Features.Companies.Queries;

public record GetCompaniesWithPaginationQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null) : IRequest<Result<PaginatedList<CompanyDto>>>;

public class GetCompaniesWithPaginationQueryHandler : IRequestHandler<GetCompaniesWithPaginationQuery, Result<PaginatedList<CompanyDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCompaniesWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<CompanyDto>>> Handle(GetCompaniesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Companies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(c => 
                c.Name.ToLower().Contains(searchTerm) || 
                (c.Domain != null && c.Domain.ToLower().Contains(searchTerm)) ||
                (c.Industry != null && c.Industry.ToLower().Contains(searchTerm)));
        }

        query = query.OrderBy(c => c.Name);

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.Domain,
                c.Industry,
                c.Description,
                c.EmployeeCount,
                c.Website,
                c.Address,
                c.City,
                c.Country,
                c.CreatedAt,
                c.ModifiedAt))
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<CompanyDto>(items, count, request.PageNumber, request.PageSize);

        return Result<PaginatedList<CompanyDto>>.Success(paginatedList);
    }
}
