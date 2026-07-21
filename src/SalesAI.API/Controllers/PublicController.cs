using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MediatR;
using SalesAI.Application.Features.Public.Commands;
using SalesAI.Application.Features.Public.Models;

namespace SalesAI.API.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/public")]
public class PublicController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submit a demo request (creates a lead in the system)
    /// </summary>
    [HttpPost("leads")]
    [EnableRateLimiting("PublicLeadLimit")]
    public async Task<ActionResult<PublicLeadResponse>> SubmitDemoRequest([FromBody] CreatePublicLeadCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return Created(string.Empty, result.Data);
    }

    /// <summary>
    /// Submit a contact form (general, sales, or support inquiry)
    /// </summary>
    [HttpPost("contact")]
    [EnableRateLimiting("PublicContactLimit")]
    public async Task<ActionResult<ContactSubmissionResponse>> SubmitContactForm([FromBody] CreateContactSubmissionCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return Created(string.Empty, result.Data);
    }

    /// <summary>
    /// Subscribe to the newsletter
    /// </summary>
    [HttpPost("newsletter")]
    [EnableRateLimiting("PublicNewsletterLimit")]
    public async Task<ActionResult<NewsletterResponse>> SubscribeNewsletter([FromBody] SubscribeNewsletterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(new { result.Message, result.Errors });

        return Ok(result.Data);
    }
}
