using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Entities;

public class AutomationLog : BaseEntity
{
    public string WorkflowName { get; set; } = null!;
    public string TriggerEvent { get; set; } = null!;
    public string Status { get; set; } = "Started";
    public int StepsCompleted { get; set; }
    public int TotalSteps { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public int? ExecutionTimeMs { get; set; }
}
