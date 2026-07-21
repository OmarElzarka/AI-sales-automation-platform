using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Deals.Models;

public record DealDto(
    Guid Id,
    string Title,
    decimal Value,
    string Currency,
    DealStage Stage,
    string StageName,
    int Probability,
    DateTime? ExpectedCloseDate,
    DateTime? ActualCloseDate,
    string? LostReason,
    Guid? LeadId,
    string? LeadName,
    Guid? CompanyId,
    string? CompanyName,
    Guid OwnerId,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
