using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.AI.Queries;

public record GenerateStandaloneEmailQuery(
    string RecipientEmail, 
    string Tone = "Professional") : IRequest<Result<GeneratedEmailResult>>;

public class GenerateStandaloneEmailQueryHandler : IRequestHandler<GenerateStandaloneEmailQuery, Result<GeneratedEmailResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAIService _aiService;

    public GenerateStandaloneEmailQueryHandler(IApplicationDbContext context, IAIService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<Result<GeneratedEmailResult>> Handle(GenerateStandaloneEmailQuery request, CancellationToken cancellationToken)
    {
        var lead = await _context.Leads
            .Include(l => l.Company)
            .FirstOrDefaultAsync(l => l.Email.ToLower() == request.RecipientEmail.ToLower(), cancellationToken);

        var firstName = lead?.FirstName ?? "there";
        var lastName = lead?.LastName ?? "";
        var companyName = lead?.Company?.Name ?? "your company";

        var ourProductName = "SalesAI Platform";
        var ourProductDescription = "An AI-powered Sales Automation Platform that acts as a copilot to qualify leads and automate outreach.";

        var painPoints = new List<string> { "Manual data entry", "Low email open rates", "Time wasted on unqualified leads" };

        var context = new EmailGenerationContext(
            firstName,
            lastName,
            lead?.JobTitle,
            companyName,
            lead?.Company?.Industry,
            painPoints,
            ourProductName,
            ourProductDescription,
            request.Tone,
            "Schedule a discovery call"
        );

        var result = await _aiService.GenerateEmailAsync(context, cancellationToken);

        return Result<GeneratedEmailResult>.Success(result);
    }
}
