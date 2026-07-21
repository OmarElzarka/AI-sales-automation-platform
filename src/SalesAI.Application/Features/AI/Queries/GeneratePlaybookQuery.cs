using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.AI.Queries;

public record GeneratePlaybookQuery(Guid LeadId) : IRequest<Result<SalesPlaybookResult>>;

public class GeneratePlaybookQueryHandler : IRequestHandler<GeneratePlaybookQuery, Result<SalesPlaybookResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAIService _aiService;

    public GeneratePlaybookQueryHandler(IApplicationDbContext context, IAIService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<Result<SalesPlaybookResult>> Handle(GeneratePlaybookQuery request, CancellationToken cancellationToken)
    {
        var lead = await _context.Leads
            .Include(l => l.Company)
            .Include(l => l.Notes)
            .Include(l => l.Activities)
            .FirstOrDefaultAsync(l => l.Id == request.LeadId, cancellationToken);

        if (lead == null)
            return Result<SalesPlaybookResult>.Failure("Lead not found.");

        // Normally, we might do a ResearchCompany pass first and pass the summary here.
        var context = new PlaybookContext(
            lead.FirstName,
            lead.LastName,
            lead.JobTitle,
            lead.Company?.Name ?? "Unknown Company",
            lead.Company?.Industry,
            lead.Company?.EmployeeCount,
            lead.ScoreCategory?.ToString(),
            lead.ScoreNumeric,
            lead.Status.ToString(),
            lead.Source.ToString(),
            lead.Activities.OrderByDescending(a => a.CreatedAt).Take(5).Select(a => $"{a.Type}: {a.Title}").ToList(),
            lead.Notes.OrderByDescending(n => n.CreatedAt).Take(5).Select(n => n.Content).ToList(),
            "Pending Research" // Placeholder
        );

        var result = await _aiService.GeneratePlaybookAsync(context, cancellationToken);

        return Result<SalesPlaybookResult>.Success(result);
    }
}
