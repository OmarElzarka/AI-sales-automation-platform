using MediatR;
using Microsoft.Extensions.Logging;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Outreach.Commands;

public record SendEmailCommand(string To, string Subject, string Body, string? Tone) : IRequest<Result<bool>>;

public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Result<bool>>
{
    private readonly ILogger<SendEmailCommandHandler> _logger;

    public SendEmailCommandHandler(ILogger<SendEmailCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        // Mock email sending process
        _logger.LogInformation("--- SENDING EMAIL ---");
        _logger.LogInformation("To: {To}", request.To);
        _logger.LogInformation("Subject: {Subject}", request.Subject);
        _logger.LogInformation("Body: \n{Body}", request.Body);
        _logger.LogInformation("---------------------");

        // Simulate network delay
        await Task.Delay(500, cancellationToken);

        return Result<bool>.Success(true);
    }
}
