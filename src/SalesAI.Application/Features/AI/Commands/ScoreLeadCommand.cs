using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.AI.Commands;

public record ScoreLeadCommand(Guid LeadId) : IRequest<Result<LeadScoreResult>>;

public class ScoreLeadCommandHandler : IRequestHandler<ScoreLeadCommand, Result<LeadScoreResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAIService _aiService;

    public ScoreLeadCommandHandler(IApplicationDbContext context, IAIService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<Result<LeadScoreResult>> Handle(ScoreLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _context.Leads
            .Include(l => l.Company)
            .Include(l => l.Notes)
            .Include(l => l.Activities)
            .FirstOrDefaultAsync(l => l.Id == request.LeadId, cancellationToken);

        if (lead == null)
            return Result<LeadScoreResult>.Failure("Lead not found.");

        var context = new LeadScoringContext(
            lead.FirstName,
            lead.LastName,
            lead.Email,
            lead.JobTitle,
            lead.Company?.Name,
            lead.Company?.Industry,
            lead.Company?.EmployeeCount,
            lead.Source.ToString(),
            lead.Status.ToString(),
            lead.CreatedAt,
            lead.Activities.OrderByDescending(a => a.CreatedAt).FirstOrDefault()?.CreatedAt,
            lead.Notes.OrderByDescending(n => n.CreatedAt).Take(5).Select(n => n.Content).ToList(),
            lead.Activities.OrderByDescending(a => a.CreatedAt).Take(5).Select(a => $"{a.Type}: {a.Title}").ToList()
        );

        var result = await _aiService.ScoreLeadAsync(context, cancellationToken);

        if (Enum.TryParse<LeadScoreCategory>(result.Category, true, out var category))
        {
            lead.UpdateScore(category, result.NumericScore, result.Reasoning);

            var history = new LeadScoreHistory
            {
                LeadId = lead.Id,
                Category = category,
                NumericScore = result.NumericScore,
                Reasoning = result.Reasoning,
                FactorsJson = JsonSerializer.Serialize(result.Factors),
                Model = "Gemini-2.5-Flash"
            };

            _context.LeadScoreHistory.Add(history);
            
            // Add activity log
            _context.Activities.Add(Activity.CreateForLead(lead.Id, ActivityType.LeadScored, "AI Lead Scored", $"Score: {result.NumericScore}/100 ({category})"));

            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            return Result<LeadScoreResult>.Failure($"AI returned invalid category: {result.Category}");
        }

        return Result<LeadScoreResult>.Success(result);
    }
}
