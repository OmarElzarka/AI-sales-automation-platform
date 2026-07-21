using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Contacts.Commands;
using SalesAI.Application.Features.Contacts.Models;
using SalesAI.Application.Features.Contacts.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ContactDto>>> GetContacts([FromQuery] GetContactsWithPaginationQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetContact(Guid id)
    {
        var result = await _mediator.Send(new GetContactByIdQuery(id));
        
        if (!result.Succeeded)
            return NotFound(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateContact([FromBody] CreateContactCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return CreatedAtAction(nameof(GetContact), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(Guid id, [FromBody] UpdateContactCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { Message = "ID mismatch." });

        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(Guid id)
    {
        var result = await _mediator.Send(new DeleteContactCommand(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return NoContent();
    }
}
