using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.AI.Queries;

public record ResearchCompanyQuery(Guid CompanyId) : IRequest<Result<CompanyResearchResult>>;

public class ResearchCompanyQueryHandler : IRequestHandler<ResearchCompanyQuery, Result<CompanyResearchResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAIService _aiService;

    public ResearchCompanyQueryHandler(IApplicationDbContext context, IAIService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<Result<CompanyResearchResult>> Handle(ResearchCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken);

        if (company == null)
            return Result<CompanyResearchResult>.Failure("Company not found.");

        // Hardcode product description for demo, normally this would be in appsettings or database
        var ourProductDescription = "AI-powered Sales Automation Platform that helps sales teams qualify leads, automate repetitive tasks, generate personalized outreach, and provide AI-driven sales insights.";

        var context = new CompanyResearchContext(
            company.Name,
            company.Website,
            company.Description,
            company.Industry,
            ourProductDescription
        );

        var result = await _aiService.ResearchCompanyAsync(context, cancellationToken);

        return Result<CompanyResearchResult>.Success(result);
    }
}
