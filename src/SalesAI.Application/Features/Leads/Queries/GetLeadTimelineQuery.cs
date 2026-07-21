using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

using SalesAI.Application.Features.Leads.Models;

namespace SalesAI.Application.Features.Leads.Queries;

public record GetLeadTimelineQuery(Guid LeadId) : IRequest<Result<List<TimelineItemDto>>>;

public class GetLeadTimelineQueryHandler : IRequestHandler<GetLeadTimelineQuery, Result<List<TimelineItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLeadTimelineQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<TimelineItemDto>>> Handle(GetLeadTimelineQuery request, CancellationToken cancellationToken)
    {
        var timeline = new List<TimelineItemDto>();

        // 1. Notes
        var notes = await _context.Notes
            .Include(n => n.Author)
            .Where(n => n.LeadId == request.LeadId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        timeline.AddRange(notes.Select(n => new TimelineItemDto(
            n.Id,
            "Note",
            "Note Added",
            n.Content,
            n.CreatedAt,
            $"{n.Author.FirstName} {n.Author.LastName}")));

        // 2. Activities
        var activities = await _context.Activities
            .Include(a => a.PerformedBy)
            .Where(a => a.LeadId == request.LeadId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        timeline.AddRange(activities.Select(a => new TimelineItemDto(
            a.Id,
            "Activity",
            a.Title,
            a.Description,
            a.CreatedAt,
            a.PerformedBy != null ? $"{a.PerformedBy.FirstName} {a.PerformedBy.LastName}" : "System")));

        // 3. Tasks
        var tasks = await _context.SalesTasks
            .Include(t => t.AssignedTo)
            .Where(t => t.LeadId == request.LeadId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        timeline.AddRange(tasks.Select(t => new TimelineItemDto(
            t.Id,
            "Task",
            $"Task Created: {t.Title}",
            t.Description,
            t.CreatedAt,
            $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}")));

        var completedTasks = tasks.Where(t => t.CompletedAt.HasValue);
        timeline.AddRange(completedTasks.Select(t => new TimelineItemDto(
            t.Id,
            "Task Completed",
            $"Task Completed: {t.Title}",
            t.Description,
            t.CompletedAt!.Value,
            $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}")));

        // 4. Deal Stage History
        var deals = await _context.Deals
            .Where(d => d.LeadId == request.LeadId)
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        if (deals.Any())
        {
            var dealHistories = await _context.DealStageHistory
                .Where(h => deals.Contains(h.DealId))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            timeline.AddRange(dealHistories.Select(h => new TimelineItemDto(
                h.Id,
                "DealStageChange",
                $"Deal Stage Changed to {h.ToStage}",
                $"Previous stage: {h.FromStage}",
                h.ChangedAt,
                h.ChangedBy)));
        }

        // Sort descending by timestamp
        var sortedTimeline = timeline.OrderByDescending(t => t.Timestamp).ToList();

        return Result<List<TimelineItemDto>>.Success(sortedTimeline);
    }
}
