using System.Text.Json;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;
using SalesAI.Domain.Events;

namespace SalesAI.Application.Features.Leads.Consumers;

public class LeadCreatedConsumer : IConsumer<LeadCreatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<LeadCreatedConsumer> _logger;
    private readonly IWigoloService _wigoloService;
    private readonly IAIService _aiService;

    public LeadCreatedConsumer(
        IApplicationDbContext context, 
        ILogger<LeadCreatedConsumer> logger,
        IWigoloService wigoloService,
        IAIService aiService)
    {
        _context = context;
        _logger = logger;
        _wigoloService = wigoloService;
        _aiService = aiService;
    }

    public async Task Consume(ConsumeContext<LeadCreatedEvent> context)
    {
        var leadId = context.Message.LeadId;
        _logger.LogInformation("🤖 AI Competitive Intelligence triggered for Lead {LeadId}", leadId);

        var lead = await _context.Leads
            .Include(l => l.Company)
            .FirstOrDefaultAsync(l => l.Id == leadId);

        if (lead == null) return;

        try
        {
            lead.ResearchStatus = ResearchStatus.InProgress;
            await _context.SaveChangesAsync();

            // 1. Research Company via Wigolo
            string companyResearch = "{}";
            if (lead.Company != null)
            {
                _logger.LogInformation("Wigolo researching company {CompanyName}", lead.Company.Name);
                companyResearch = await _wigoloService.ResearchCompanyAsync(lead.Company.Name, lead.Company.Website);
            }

            // 2. Research Lead via Wigolo
            _logger.LogInformation("Wigolo researching lead {LeadName}", lead.FullName);
            string leadResearch = await _wigoloService.ResearchLeadAsync(lead.FirstName, lead.LastName, lead.Company?.Name ?? "", lead.JobTitle);

            // 3. Synthesize via Gemini
            var aiContext = new CompetitiveIntelligenceContext(companyResearch, leadResearch);
            var result = await _aiService.GenerateCompetitiveIntelligenceAsync(aiContext);

            // 4. Save to Database
            var aiContent = new AIGeneratedContent
            {
                Type = AIContentType.CompetitiveIntelligence,
                ContentJson = JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                LeadId = leadId,
                RequestedById = lead.AssignedToId,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.AIGeneratedContent.Add(aiContent);

            // 4.5 Generate Email
            _logger.LogInformation("Generating personalized email for lead {LeadName}", lead.FullName);
            var emailContext = new EmailGenerationContext(
                lead.FirstName,
                lead.LastName,
                lead.JobTitle,
                lead.Company?.Name ?? "Your Company",
                lead.Company?.Industry,
                result.PotentialPainPoints ?? new List<string>(),
                "SalesAI",
                "An AI-powered sales automation platform that streamlines workflows and increases win rates.",
                "Professional, empathetic, and value-driven",
                "Schedule a 15-minute introductory call"
            );
            
            var emailResult = await _aiService.GenerateEmailAsync(emailContext);
            var emailContent = new AIGeneratedContent
            {
                Type = AIContentType.Email,
                ContentJson = JsonSerializer.Serialize(emailResult, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                LeadId = leadId,
                RequestedById = lead.AssignedToId,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.AIGeneratedContent.Add(emailContent);
            
            // Also update the lead score denormalized properties
            lead.ScoreNumeric = result.LeadScore;
            lead.ScoreCategory = result.LeadTemperature switch
            {
                "Hot" => LeadScoreCategory.Hot,
                "Warm" => LeadScoreCategory.Warm,
                _ => LeadScoreCategory.Cold
            };

            lead.ResearchStatus = ResearchStatus.Completed;
            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ AI Competitive Intelligence completed for Lead {LeadId}", leadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ AI Competitive Intelligence failed for Lead {LeadId}", leadId);
            lead.ResearchStatus = ResearchStatus.Failed;
            await _context.SaveChangesAsync();
        }
    }
}
