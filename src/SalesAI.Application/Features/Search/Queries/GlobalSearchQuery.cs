using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Search.Queries;

public record GlobalSearchResult(string Type, string Name, string Sub, string Route);

public record GlobalSearchQuery(string Query) : IRequest<Result<List<GlobalSearchResult>>>;

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, Result<List<GlobalSearchResult>>>
{
    private readonly IApplicationDbContext _context;

    public GlobalSearchQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<GlobalSearchResult>>> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        var results = new List<GlobalSearchResult>();
        
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return Result<List<GlobalSearchResult>>.Success(results);
        }

        var q = request.Query.ToLower();

        var leads = await _context.Leads
            .Where(l => l.FirstName.ToLower().Contains(q) || l.LastName.ToLower().Contains(q) || l.CompanyName.ToLower().Contains(q))
            .Take(5)
            .ToListAsync(cancellationToken);

        var deals = await _context.Deals
            .Where(d => d.Title.ToLower().Contains(q))
            .Take(5)
            .ToListAsync(cancellationToken);

        foreach (var lead in leads)
        {
            results.Add(new GlobalSearchResult("Lead", $"{lead.FirstName} {lead.LastName}", lead.CompanyName ?? "No Company", $"/leads/{lead.Id}"));
        }

        foreach (var deal in deals)
        {
            results.Add(new GlobalSearchResult("Deal", deal.Title, $"${deal.Value}", "/deals"));
        }

        return Result<List<GlobalSearchResult>>.Success(results);
    }
}
