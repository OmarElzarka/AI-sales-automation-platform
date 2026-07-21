using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.AI.Commands;

public record SummarizeMeetingCommand(Guid DealId, string TranscriptOrNotes) : IRequest<Result<MeetingSummaryResult>>;

public class SummarizeMeetingCommandHandler : IRequestHandler<SummarizeMeetingCommand, Result<MeetingSummaryResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAIService _aiService;
    private readonly ICurrentUserService _currentUserService;

    public SummarizeMeetingCommandHandler(IApplicationDbContext context, IAIService aiService, ICurrentUserService currentUserService)
    {
        _context = context;
        _aiService = aiService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<MeetingSummaryResult>> Handle(SummarizeMeetingCommand request, CancellationToken cancellationToken)
    {
        var deal = await _context.Deals
            .Include(d => d.Lead)
            .Include(d => d.Company)
            .FirstOrDefaultAsync(d => d.Id == request.DealId, cancellationToken);

        if (deal == null)
            return Result<MeetingSummaryResult>.Failure("Deal not found.");

        var attendees = deal.Lead != null ? $"{deal.Lead.FirstName} {deal.Lead.LastName}" : "Customer";
        
        var context = new MeetingSummaryContext(
            DateTime.UtcNow.ToString("yyyy-MM-dd"),
            attendees,
            deal.Title,
            deal.Stage.ToString(),
            deal.Value,
            request.TranscriptOrNotes
        );

        var result = await _aiService.SummarizeMeetingAsync(context, cancellationToken);

        // Save summary as a Note
        var note = new Note
        {
            DealId = deal.Id,
            LeadId = deal.LeadId,
            AuthorId = _currentUserService.UserId,
            Content = $"**AI Meeting Summary**\n\n{result.Summary}\n\n*Sentiment: {result.Sentiment} | Impact: {result.DealImpact}*"
        };

        _context.Notes.Add(note);

        // Optional: Create Tasks for Action Items
        foreach (var item in result.ActionItems)
        {
            var task = new SalesTask
            {
                DealId = deal.Id,
                LeadId = deal.LeadId,
                Title = $"[Action Item] {item.Action}",
                Description = $"Owner: {item.Owner}\nDeadline: {item.Deadline}",
                Type = TaskType.FollowUp,
                Priority = TaskPriority.High,
                Status = SalesTaskStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(2), // Rough estimate
                AssignedToId = _currentUserService.UserId // Assign to current user for now
            };
            _context.SalesTasks.Add(task);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<MeetingSummaryResult>.Success(result);
    }
}
