using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OutreachController : ControllerBase
{
    [HttpPost("email/send")]
    public IActionResult SendEmail([FromBody] SendEmailRequest request)
    {
        // In a real system, this would queue a message to RabbitMQ to send via SMTP/SendGrid.
        // For now, we simulate a successful send to fulfill the frontend integration.
        return Ok(new { Message = "Email sent successfully" });
    }
}

public class SendEmailRequest
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Tone { get; set; }
}
