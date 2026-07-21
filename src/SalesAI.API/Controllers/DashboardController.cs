using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Features.Dashboard.Models;
using SalesAI.Application.Features.Dashboard.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("data")]
    public async Task<ActionResult<DashboardDataDto>> GetDashboardData([FromQuery] Guid? userId = null)
    {
        var result = await _mediator.Send(new GetDashboardDataQuery(userId));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }

    [HttpGet("activity")]
    public async Task<ActionResult<List<ActivityDto>>> GetRecentActivity([FromQuery] int take = 10, [FromQuery] Guid? userId = null)
    {
        var result = await _mediator.Send(new GetRecentActivityQuery(take, userId));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }
}
