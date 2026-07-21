using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Leads.Commands;
using SalesAI.Application.Features.Leads.Models;
using SalesAI.Application.Features.Leads.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LeadsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<LeadDto>>> GetLeads([FromQuery] GetLeadsWithPaginationQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeadDto>> GetLead(Guid id)
    {
        var result = await _mediator.Send(new GetLeadByIdQuery(id));
        
        if (!result.Succeeded)
            return NotFound(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateLead([FromBody] CreateLeadCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return CreatedAtAction(nameof(GetLead), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLead(Guid id, [FromBody] UpdateLeadCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { Message = "ID mismatch." });

        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLead(Guid id)
    {
        var result = await _mediator.Send(new DeleteLeadCommand(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return NoContent();
    }

    [HttpGet("duplicates")]
    public async Task<ActionResult<List<LeadDto>>> GetDuplicates([FromQuery] GetPotentialDuplicateLeadsQuery query)
    {
        var result = await _mediator.Send(query);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost("import")]
    public async Task<ActionResult<ImportLeadsResult>> ImportLeads(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Message = "File is empty or not provided." });

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out var assignedToId))
            return Unauthorized();

        var result = await _mediator.Send(new ImportLeadsCommand(fileBytes, assignedToId));

        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("{id}/timeline")]
    public async Task<ActionResult<List<TimelineItemDto>>> GetLeadTimeline(Guid id)
    {
        var result = await _mediator.Send(new GetLeadTimelineQuery(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }
}
