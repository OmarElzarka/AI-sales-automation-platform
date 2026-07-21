using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Dashboard.Models;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Dashboard.Queries;

public record GetDashboardDataQuery(Guid? UserId = null) : IRequest<Result<DashboardDataDto>>;

public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, Result<DashboardDataDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public GetDashboardDataQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<DashboardDataDto>> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = request.UserId.HasValue ? $"dashboard_data_{request.UserId.Value}" : "dashboard_data_all";
        
        var cachedResult = await _cache.GetAsync<DashboardDataDto>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            return Result<DashboardDataDto>.Success(cachedResult);
        }

        var leadsQuery = _context.Leads.AsNoTracking();
        var dealsQuery = _context.Deals.AsNoTracking();
        var tasksQuery = _context.SalesTasks.AsNoTracking();

        if (request.UserId.HasValue)
        {
            leadsQuery = leadsQuery.Where(l => l.AssignedToId == request.UserId.Value);
            dealsQuery = dealsQuery.Where(d => d.OwnerId == request.UserId.Value);
            tasksQuery = tasksQuery.Where(t => t.AssignedToId == request.UserId.Value);
        }

        // 1. KPIs
        var totalLeads = await leadsQuery.CountAsync(cancellationToken);
        var totalDeals = await dealsQuery.CountAsync(cancellationToken);
        
        var wonDeals = await dealsQuery.Where(d => d.Stage == DealStage.Won).ToListAsync(cancellationToken);
        var totalRevenue = wonDeals.Sum(d => d.Value);
        
        var lostDealsCount = await dealsQuery.CountAsync(d => d.Stage == DealStage.Lost, cancellationToken);
        var wonDealsCount = wonDeals.Count;
        
        decimal winRate = 0;
        var completedDeals = wonDealsCount + lostDealsCount;
        if (completedDeals > 0)
        {
            winRate = (decimal)wonDealsCount / completedDeals * 100;
        }

        var activeTasks = await tasksQuery.CountAsync(t => t.Status != SalesTaskStatus.Completed && t.Status != SalesTaskStatus.Cancelled, cancellationToken);

        var kpis = new DashboardKpisDto(totalLeads, totalDeals, totalRevenue, activeTasks, Math.Round(winRate, 2));

        // 2. Pipeline Funnel
        var funnelData = await dealsQuery
            .GroupBy(d => d.Stage)
            .Select(g => new FunnelStageDto(g.Key.ToString(), g.Count(), g.Sum(d => d.Value)))
            .ToListAsync(cancellationToken);

        // Ensure all stages are present even if count is 0
        var allStages = Enum.GetValues<DealStage>();
        var fullFunnel = allStages.Select(stage => 
        {
            var existing = funnelData.FirstOrDefault(f => f.StageName == stage.ToString());
            return existing ?? new FunnelStageDto(stage.ToString(), 0, 0);
        }).ToList();

        // 3. Lead Source Breakdown
        var sourceData = await leadsQuery
            .GroupBy(l => l.Source)
            .Select(g => new LeadSourceBreakdownDto(g.Key.ToString(), g.Count()))
            .ToListAsync(cancellationToken);

        var dashboardData = new DashboardDataDto(kpis, fullFunnel, sourceData);

        await _cache.SetAsync(cacheKey, dashboardData, TimeSpan.FromMinutes(5), cancellationToken);

        return Result<DashboardDataDto>.Success(dashboardData);
    }
}
