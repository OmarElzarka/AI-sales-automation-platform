using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Features.AI.Jobs;
using SalesAI.Domain.Events;

namespace SalesAI.Application.Features.Leads.EventHandlers;

public class LeadCreatedEventHandler : INotificationHandler<LeadCreatedEvent>
{
    private readonly IBackgroundJobService _backgroundJobService;

    public LeadCreatedEventHandler(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }

    public async Task Handle(LeadCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Enqueue background job to score lead and perform research
        _backgroundJobService.Enqueue<LeadAutomationJob>(job => job.ProcessNewLeadAsync(notification.LeadId, null));
    }
}
