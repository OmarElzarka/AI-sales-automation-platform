namespace SalesAI.Application.Features.Contacts.Models;

public record ContactDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? JobTitle,
    bool IsPrimary,
    Guid CompanyId,
    string? CompanyName,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
