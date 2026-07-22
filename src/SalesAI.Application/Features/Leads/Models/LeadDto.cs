namespace SalesAI.Application.Features.Leads.Models;

public record LeadDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? CompanyName,
    string? JobTitle,
    string Status,
    string Source,
    Guid AssignedToId,
    Guid? CompanyId,
    int? ScoreNumeric,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    string ResearchStatus,
    string? CompetitiveIntelligenceJson = null,
    string? EmailDraftJson = null);
