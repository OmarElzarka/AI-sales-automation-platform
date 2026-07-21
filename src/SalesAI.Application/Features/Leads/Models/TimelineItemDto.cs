namespace SalesAI.Application.Features.Leads.Models;

public record TimelineItemDto(
    Guid Id,
    string Type, // "Note", "Activity", "Task", "DealStageChange"
    string Title,
    string? Description,
    DateTime Timestamp,
    string? PerformedBy);
