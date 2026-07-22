using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Leads.Models;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Leads.Queries;

public record GetLeadsWithPaginationQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null,
    string? SearchTerm = null) : IRequest<Result<PaginatedList<LeadDto>>>;

public class GetLeadsWithPaginationQueryHandler : IRequestHandler<GetLeadsWithPaginationQuery, Result<PaginatedList<LeadDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLeadsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<LeadDto>>> Handle(GetLeadsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Leads.Include(l => l.Company).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<LeadStatus>(request.Status, true, out var status))
        {
            query = query.Where(l => l.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(l => 
                l.FirstName.ToLower().Contains(searchTerm) || 
                l.LastName.ToLower().Contains(searchTerm) || 
                l.Email.ToLower().Contains(searchTerm) || 
                (l.Company != null && l.Company.Name.ToLower().Contains(searchTerm)));
        }

        query = query.OrderByDescending(l => l.CreatedAt);

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(lead => new LeadDto(
                lead.Id,
                lead.FirstName,
                lead.LastName,
                lead.Email,
                lead.Phone,
                lead.Company != null ? lead.Company.Name : null,
                lead.JobTitle,
                lead.Status.ToString(),
                lead.Source.ToString(),
                lead.AssignedToId,
                lead.CompanyId,
                lead.ScoreNumeric,
                lead.CreatedAt,
                lead.ModifiedAt,
                lead.ResearchStatus.ToString(),
                null,
                null))
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<LeadDto>(items, count, request.PageNumber, request.PageSize);

        return Result<PaginatedList<LeadDto>>.Success(paginatedList);
    }
}
