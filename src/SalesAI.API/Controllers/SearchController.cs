using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Features.Search.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GlobalSearch([FromQuery] string q)
    {
        var result = await _mediator.Send(new GlobalSearchQuery(q));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return Ok(result.Data);
    }
}
