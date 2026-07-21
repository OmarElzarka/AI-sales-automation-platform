using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Deals.Models;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Deals.Queries;

public record GetPipelineDealsQuery : IRequest<Result<Dictionary<string, List<DealDto>>>>;

public class GetPipelineDealsQueryHandler : IRequestHandler<GetPipelineDealsQuery, Result<Dictionary<string, List<DealDto>>>>
{
    private readonly IApplicationDbContext _context;

    public GetPipelineDealsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Dictionary<string, List<DealDto>>>> Handle(GetPipelineDealsQuery request, CancellationToken cancellationToken)
    {
        // Get all deals that are not Lost or Won, or we might want to group by all stages
        var deals = await _context.Deals
            .Include(d => d.Lead)
            .Include(d => d.Company)
            .AsNoTracking()
            .OrderByDescending(d => d.ExpectedCloseDate ?? d.CreatedAt)
            .Select(deal => new DealDto(
                deal.Id,
                deal.Title,
                deal.Value,
                deal.Currency,
                deal.Stage,
                deal.Stage.ToString(),
                deal.Probability,
                deal.ExpectedCloseDate,
                deal.ActualCloseDate,
                deal.LostReason,
                deal.LeadId,
                deal.Lead != null ? $"{deal.Lead.FirstName} {deal.Lead.LastName}" : null,
                deal.CompanyId,
                deal.Company != null ? deal.Company.Name : null,
                deal.OwnerId,
                deal.CreatedAt,
                deal.ModifiedAt))
            .ToListAsync(cancellationToken);

        // Initialize dictionary with all stages
        var pipeline = Enum.GetNames<DealStage>()
            .ToDictionary(stage => stage, stage => new List<DealDto>());

        // Populate deals into their respective stages
        foreach (var deal in deals)
        {
            pipeline[deal.StageName].Add(deal);
        }

        return Result<Dictionary<string, List<DealDto>>>.Success(pipeline);
    }
}
