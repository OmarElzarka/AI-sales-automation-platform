using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Companies.Commands;
using SalesAI.Application.Features.Companies.Models;
using SalesAI.Application.Features.Companies.Queries;

namespace SalesAI.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<CompanyDto>>> GetCompanies([FromQuery] GetCompaniesWithPaginationQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(Guid id)
    {
        var result = await _mediator.Send(new GetCompanyByIdQuery(id));
        
        if (!result.Succeeded)
            return NotFound(new { result.Message });

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateCompany([FromBody] CreateCompanyCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return CreatedAtAction(nameof(GetCompany), new { id = result.Data }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        if (id != command.Id)
            return BadRequest(new { Message = "ID mismatch." });

        var result = await _mediator.Send(command);
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        var result = await _mediator.Send(new DeleteCompanyCommand(id));
        
        if (!result.Succeeded)
            return BadRequest(new { result.Message });

        return NoContent();
    }
}
