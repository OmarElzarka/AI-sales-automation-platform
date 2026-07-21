using MediatR;
using SalesAI.Application.Features.AI.Commands;
using SalesAI.Application.Features.AI.Queries;

namespace SalesAI.Application.Features.AI.Jobs;

public class LeadAutomationJob
{
    private readonly IMediator _mediator;

    public LeadAutomationJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ProcessNewLeadAsync(Guid leadId, Guid? companyId)
    {
        // 1. Score the lead
        await _mediator.Send(new ScoreLeadCommand(leadId));

        // 2. If the lead has a company, run company research
        if (companyId.HasValue)
        {
            await _mediator.Send(new ResearchCompanyQuery(companyId.Value));
            // In a real scenario, we might save the research result back to the Company entity or as a Note attached to the Company/Lead.
        }
    }
}
