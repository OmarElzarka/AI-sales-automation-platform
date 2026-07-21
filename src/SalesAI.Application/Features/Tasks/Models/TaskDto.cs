using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Tasks.Models;

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskType Type,
    string TypeName,
    TaskPriority Priority,
    string PriorityName,
    SalesTaskStatus Status,
    string StatusName,
    DateTime DueDate,
    DateTime? CompletedAt,
    bool ReminderSent,
    bool IsOverdue,
    Guid AssignedToId,
    Guid? LeadId,
    Guid? DealId,
    Guid? ContactId,
    DateTime CreatedAt,
    DateTime? ModifiedAt);
