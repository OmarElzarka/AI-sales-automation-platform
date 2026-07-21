using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Dashboard.Models;

public record DashboardKpisDto(
    int TotalLeads,
    int TotalDeals,
    decimal TotalRevenue,
    int ActiveTasks,
    decimal WinRatePercentage);

public record FunnelStageDto(string StageName, int Count, decimal TotalValue);

public record LeadSourceBreakdownDto(string SourceName, int Count);

public record DashboardDataDto(
    DashboardKpisDto Kpis,
    List<FunnelStageDto> PipelineFunnel,
    List<LeadSourceBreakdownDto> LeadSources);
