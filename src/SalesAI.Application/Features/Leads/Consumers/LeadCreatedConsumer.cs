using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Features.AI.Commands;
using SalesAI.Application.Features.AI.Queries;
using SalesAI.Domain.Events;

namespace SalesAI.Application.Features.Leads.Consumers;

public class LeadCreatedConsumer : IConsumer<LeadCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<LeadCreatedConsumer> _logger;

    public LeadCreatedConsumer(IMediator mediator, IApplicationDbContext context, ILogger<LeadCreatedConsumer> logger)
    {
        _mediator = mediator;
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LeadCreatedEvent> context)
    {
        var leadId = context.Message.LeadId;
        _logger.LogInformation("🤖 AI Automation triggered for Lead {LeadId}", leadId);

        // 1. Score the lead via AI
        try
        {
            var scoreResult = await _mediator.Send(new ScoreLeadCommand(leadId));
            if (scoreResult.Succeeded)
            {
                _logger.LogInformation("✅ Lead {LeadId} scored: {Score}/100 ({Category})", 
                    leadId, scoreResult.Data!.NumericScore, scoreResult.Data.Category);
            }
            else
            {
                _logger.LogWarning("⚠️ Lead scoring failed for {LeadId}: {Message}", leadId, scoreResult.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Lead scoring error for {LeadId}", leadId);
        }

        // 2. Research the company if it has one
        try
        {
            var lead = await _context.Leads.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == leadId);

            if (lead?.CompanyId != null)
            {
                var researchResult = await _mediator.Send(new ResearchCompanyQuery(lead.CompanyId.Value));
                if (researchResult.Succeeded)
                {
                    _logger.LogInformation("✅ Company research completed for Lead {LeadId}", leadId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Company research error for Lead {LeadId}", leadId);
        }

        _logger.LogInformation("🤖 AI Automation completed for Lead {LeadId}", leadId);
    }
}
