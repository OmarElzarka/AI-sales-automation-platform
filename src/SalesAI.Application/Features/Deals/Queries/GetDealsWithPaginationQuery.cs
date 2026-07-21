using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Deals.Models;

namespace SalesAI.Application.Features.Deals.Queries;

public record GetDealsWithPaginationQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CompanyId = null,
    Guid? LeadId = null,
    string? SearchTerm = null) : IRequest<Result<PaginatedList<DealDto>>>;

public class GetDealsWithPaginationQueryHandler : IRequestHandler<GetDealsWithPaginationQuery, Result<PaginatedList<DealDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetDealsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<DealDto>>> Handle(GetDealsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Deals
            .Include(d => d.Lead)
            .Include(d => d.Company)
            .AsNoTracking()
            .AsQueryable();

        if (request.CompanyId.HasValue)
        {
            query = query.Where(d => d.CompanyId == request.CompanyId.Value);
        }

        if (request.LeadId.HasValue)
        {
            query = query.Where(d => d.LeadId == request.LeadId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(d => 
                d.Title.ToLower().Contains(searchTerm) || 
                (d.Company != null && d.Company.Name.ToLower().Contains(searchTerm)) ||
                (d.Lead != null && (d.Lead.FirstName.ToLower().Contains(searchTerm) || d.Lead.LastName.ToLower().Contains(searchTerm))));
        }

        query = query.OrderByDescending(d => d.CreatedAt);

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(deal => new DealDto(
                deal.Id,
                deal.Title,
                deal.Value,
                deal.Currency,
                deal.Stage,
                deal.Stage.ToString(),
                deal.Probability,
                deal.ExpectedCloseDate,
                deal.ActualCloseDate,
                deal.LostReason,
                deal.LeadId,
                deal.Lead != null ? $"{deal.Lead.FirstName} {deal.Lead.LastName}" : null,
                deal.CompanyId,
                deal.Company != null ? deal.Company.Name : null,
                deal.OwnerId,
                deal.CreatedAt,
                deal.ModifiedAt))
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<DealDto>(items, count, request.PageNumber, request.PageSize);

        return Result<PaginatedList<DealDto>>.Success(paginatedList);
    }
}
