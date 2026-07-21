using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Tasks.Commands;
using SalesAI.Application.Features.Tasks.Models;
using SalesAI.Application.Features.Tasks.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<TaskDto>>> GetTasks([FromQuery] GetTasksWithPaginationQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskDto>> GetTask(Guid id)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery(id));
        
        if (!result.Succeeded)
            return NotFound(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTask([FromBody] CreateTaskCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return CreatedAtAction(nameof(GetTask), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { Message = "ID mismatch." });

        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> CompleteTask(Guid id)
    {
        var result = await _mediator.Send(new CompleteTaskCommand(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var result = await _mediator.Send(new DeleteTaskCommand(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return NoContent();
    }
}
