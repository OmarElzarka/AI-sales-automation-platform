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

    [HttpGet("export-csv")]
    public IActionResult ExportCsv()
    {
        var csvContent = "Metric,Value\n" +
                         "Total Revenue,$2450000\n" +
                         "Avg Deal Size,$34000\n" +
                         "Sales Cycle Days,42\n" +
                         "Win Rate,68.5%";
        var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
        return File(bytes, "text/csv", "sales_report.csv");
    }

    [HttpGet("export-pdf")]
    public IActionResult ExportPdf()
    {
        // Dummy PDF export. A real implementation would use a library like iTextSharp or DinkToPdf.
        var pdfContent = "%PDF-1.4\n1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n" +
                         "2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n" +
                         "3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>\nendobj\n" +
                         "4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n" +
                         "5 0 obj\n<< /Length 53 >>\nstream\nBT\n/F1 12 Tf\n100 700 Td\n(Sales Report Mock PDF) Tj\nET\nendstream\nendobj\n" +
                         "trailer\n<< /Root 1 0 R >>\n%%EOF";
        var bytes = System.Text.Encoding.UTF8.GetBytes(pdfContent);
        return File(bytes, "application/pdf", "sales_report.pdf");
    }
}
