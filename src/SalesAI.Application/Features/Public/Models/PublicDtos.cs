namespace SalesAI.Application.Features.Public.Models;

public record PublicLeadResponse(
    Guid Id,
    string Message,
    string Status);

public record ContactSubmissionResponse(
    Guid Id,
    string Message);

public record NewsletterResponse(
    string Email,
    string Message);
