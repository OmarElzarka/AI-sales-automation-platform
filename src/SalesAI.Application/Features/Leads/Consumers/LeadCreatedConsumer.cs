using MassTransit;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Features.AI.Commands;
using SalesAI.Domain.Events;

namespace SalesAI.Application.Features.Leads.Consumers;

public class LeadCreatedConsumer : IConsumer<LeadCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public LeadCreatedConsumer(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task Consume(ConsumeContext<LeadCreatedEvent> context)
    {
        var leadId = context.Message.LeadId;
        
        // 1. Score the lead
        await _mediator.Send(new ScoreLeadCommand(leadId));

        // 2. We can also trigger email generation or company research here asynchronously!
    }
}
