using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Tasks.Jobs;

public class FollowUpReminderJob
{
    private readonly IApplicationDbContext _context;

    public FollowUpReminderJob(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CheckForFollowUpsAsync()
    {
        var threeDaysAgo = DateTime.UtcNow.AddDays(-3);

        // Find deals that haven't been modified in 3 days and are active
        var staleDeals = await _context.Deals
            .Where(d => d.Stage != DealStage.Won && d.Stage != DealStage.Lost)
            .Where(d => d.ModifiedAt == null ? d.CreatedAt < threeDaysAgo : d.ModifiedAt < threeDaysAgo)
            .ToListAsync();

        foreach (var deal in staleDeals)
        {
            // Check if there is already a pending follow-up task
            var hasPendingTask = await _context.SalesTasks
                .AnyAsync(t => t.DealId == deal.Id && t.Status != SalesTaskStatus.Completed && t.Status != SalesTaskStatus.Cancelled);

            if (!hasPendingTask)
            {
                var task = new SalesTask
                {
                    Title = $"Follow up on deal: {deal.Title}",
                    Description = "This deal has been inactive for 3 days. Follow up with the lead.",
                    Type = TaskType.FollowUp,
                    Priority = TaskPriority.Medium,
                    DueDate = DateTime.UtcNow.AddDays(1),
                    AssignedToId = deal.OwnerId,
                    DealId = deal.Id,
                    LeadId = deal.LeadId,
                    Status = SalesTaskStatus.Pending
                };

                _context.SalesTasks.Add(task);
            }
        }

        await _context.SaveChangesAsync();
    }
}
