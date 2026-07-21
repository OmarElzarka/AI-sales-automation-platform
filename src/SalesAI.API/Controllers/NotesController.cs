using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Features.Notes.Commands;
using SalesAI.Application.Features.Notes.Models;
using SalesAI.Application.Features.Notes.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<NoteDto>>> GetNotes([FromQuery] Guid? leadId, [FromQuery] Guid? dealId)
    {
        var result = await _mediator.Send(new GetNotesByEntityQuery(leadId, dealId));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateNote([FromBody] CreateNoteCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(Guid id, [FromBody] UpdateNoteCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { Message = "ID mismatch." });

        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(Guid id)
    {
        var result = await _mediator.Send(new DeleteNoteCommand(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return NoContent();
    }
}
