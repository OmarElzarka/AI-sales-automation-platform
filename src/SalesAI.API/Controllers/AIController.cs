using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Features.AI.Commands;
using SalesAI.Application.Features.AI.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly IMediator _mediator;

    public AIController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("leads/{id}/score")]
    public async Task<IActionResult> ScoreLead(Guid id)
    {
        var result = await _mediator.Send(new ScoreLeadCommand(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost("companies/{id}/research")]
    public async Task<IActionResult> ResearchCompany(Guid id)
    {
        var result = await _mediator.Send(new ResearchCompanyQuery(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost("leads/{id}/generate-email")]
    public async Task<IActionResult> GenerateEmail(Guid id, [FromQuery] string tone = "Professional", [FromQuery] string goal = "Schedule a discovery call")
    {
        var result = await _mediator.Send(new GenerateEmailQuery(id, tone, goal));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost("deals/{id}/summarize-meeting")]
    public async Task<IActionResult> SummarizeMeeting(Guid id, [FromBody] string transcriptOrNotes)
    {
        var result = await _mediator.Send(new SummarizeMeetingCommand(id, transcriptOrNotes));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost("leads/{id}/playbook")]
    public async Task<IActionResult> GeneratePlaybook(Guid id)
    {
        var result = await _mediator.Send(new GeneratePlaybookQuery(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }
}
