using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.AI.Queries;

public record GenerateEmailQuery(
    Guid LeadId, 
    string Tone = "Professional", 
    string? Goal = "Schedule a discovery call") : IRequest<Result<GeneratedEmailResult>>;

public class GenerateEmailQueryHandler : IRequestHandler<GenerateEmailQuery, Result<GeneratedEmailResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAIService _aiService;

    public GenerateEmailQueryHandler(IApplicationDbContext context, IAIService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<Result<GeneratedEmailResult>> Handle(GenerateEmailQuery request, CancellationToken cancellationToken)
    {
        var lead = await _context.Leads
            .Include(l => l.Company)
            .FirstOrDefaultAsync(l => l.Id == request.LeadId, cancellationToken);

        if (lead == null)
            return Result<GeneratedEmailResult>.Failure("Lead not found.");

        var ourProductName = "SalesAI Platform";
        var ourProductDescription = "An AI-powered Sales Automation Platform that acts as a copilot to qualify leads and automate outreach.";

        // For a real app, we'd pull pain points from Lead Notes or Company Research
        var painPoints = new List<string> { "Manual data entry", "Low email open rates", "Time wasted on unqualified leads" };

        var context = new EmailGenerationContext(
            lead.FirstName,
            lead.LastName,
            lead.JobTitle,
            lead.Company?.Name ?? "Your Company",
            lead.Company?.Industry,
            painPoints,
            ourProductName,
            ourProductDescription,
            request.Tone,
            request.Goal
        );

        var result = await _aiService.GenerateEmailAsync(context, cancellationToken);

        return Result<GeneratedEmailResult>.Success(result);
    }
}
