using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Features.AI.Jobs;
using SalesAI.Domain.Events;

namespace SalesAI.Application.Features.Leads.EventHandlers;

public class LeadCreatedEventHandler : INotificationHandler<LeadCreatedEvent>
{
    private readonly MassTransit.IPublishEndpoint _publishEndpoint;

    public LeadCreatedEventHandler(MassTransit.IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(LeadCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Publish to RabbitMQ instead of Hangfire
        await _publishEndpoint.Publish(notification, cancellationToken);
    }
}
