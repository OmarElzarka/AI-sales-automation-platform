using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Dashboard.Models;

namespace SalesAI.Application.Features.Dashboard.Queries;

public record GetRecentActivityQuery(int Take = 10, Guid? UserId = null) : IRequest<Result<List<ActivityDto>>>;

public class GetRecentActivityQueryHandler : IRequestHandler<GetRecentActivityQuery, Result<List<ActivityDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetRecentActivityQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ActivityDto>>> Handle(GetRecentActivityQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Activities
            .Include(a => a.PerformedBy)
            .Include(a => a.Lead)
            .Include(a => a.Deal)
            .AsNoTracking()
            .AsQueryable();

        if (request.UserId.HasValue)
        {
            query = query.Where(a => a.PerformedById == request.UserId.Value);
        }

        var activities = await query
            .OrderByDescending(a => a.CreatedAt)
            .Take(request.Take)
            .Select(a => new ActivityDto(
                a.Id,
                a.Type.ToString(),
                a.Title,
                a.Description,
                a.CreatedAt,
                a.PerformedBy != null ? $"{a.PerformedBy.FirstName} {a.PerformedBy.LastName}" : "System",
                a.LeadId,
                a.Lead != null ? $"{a.Lead.FirstName} {a.Lead.LastName}" : null,
                a.DealId,
                a.Deal != null ? a.Deal.Title : null))
            .ToListAsync(cancellationToken);

        return Result<List<ActivityDto>>.Success(activities);
    }
}
