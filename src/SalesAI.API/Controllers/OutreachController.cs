using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Features.Outreach.Commands;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OutreachController : ControllerBase
{
    private readonly IMediator _mediator;

    public OutreachController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("email/send")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
    {
        var result = await _mediator.Send(new SendEmailCommand(request.To, request.Subject, request.Body, request.Tone));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

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
