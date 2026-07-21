using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetReports()
    {
        // For a full implementation, we would query the database to generate real metrics.
        // As a fast-path for the integration, we return sample calculated metrics.
        var metrics = new
        {
            totalRevenue = 2450000,
            avgDealSize = 34000,
            salesCycleDays = 42,
            winRate = 68.5,
            topPerformers = new[]
            {
                new { name = "Sarah Jenkins", revenue = 850000 },
                new { name = "Mike Ross", revenue = 620000 }
            },
            revenueByMonth = new[] { 120, 150, 180, 140, 210, 245 }
        };

        var insights = new[]
        {
            "Win rate drops by 15% when sales cycles extend beyond 45 days. Consider implementing automated re-engagement triggers at day 30.",
            "Deals originating from outbound email campaigns have a 20% higher average deal size compared to inbound leads.",
            "Top performer Sarah Jenkins averages 3 extra touchpoints per deal, mostly phone calls, leading to a 85% win rate in Enterprise deals."
        };

        return Ok(new { metrics, insights });
    }
}
