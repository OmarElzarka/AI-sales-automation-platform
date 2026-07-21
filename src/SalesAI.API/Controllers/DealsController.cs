using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Deals.Commands;
using SalesAI.Application.Features.Deals.Models;
using SalesAI.Application.Features.Deals.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DealsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DealsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<DealDto>>> GetDeals([FromQuery] GetDealsWithPaginationQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DealDto>> GetDeal(Guid id)
    {
        var result = await _mediator.Send(new GetDealByIdQuery(id));
        
        if (!result.Succeeded)
            return NotFound(new { result.Message });

        return Ok(result.Data);
    }

    [HttpGet("pipeline")]
    public async Task<ActionResult<Dictionary<string, List<DealDto>>>> GetPipeline()
    {
        var result = await _mediator.Send(new GetPipelineDealsQuery());
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateDeal([FromBody] CreateDealCommand command)
    {
        // Enforce the owner is the current user if not provided?
        // Wait, command has OwnerId. 
        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return CreatedAtAction(nameof(GetDeal), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeal(Guid id, [FromBody] UpdateDealCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { Message = "ID mismatch." });

        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    [HttpPut("{id}/stage")]
    public async Task<IActionResult> MoveDealStage(Guid id, [FromBody] MoveDealStageCommand command)
    {
        if (id != command.DealId)
            return BadRequest(new { Message = "ID mismatch." });

        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeal(Guid id)
    {
        var result = await _mediator.Send(new DeleteDealCommand(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return NoContent();
    }
}
