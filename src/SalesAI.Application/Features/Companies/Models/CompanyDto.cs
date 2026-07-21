namespace SalesAI.Application.Features.Companies.Models;

public record CompanyDto(
    Guid Id,
    string Name,
    string? Domain,
    string? Industry,
    string? Description,
    int? EmployeeCount,
    string? Website,
    string? Address,
    string? City,
    string? Country,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
