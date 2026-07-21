using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Entities;

public class SalesTask : AuditableEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public TaskType Type { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public SalesTaskStatus Status { get; set; } = SalesTaskStatus.Pending;
    public DateTime DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool ReminderSent { get; set; }

    // Foreign Keys
    public Guid AssignedToId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? DealId { get; set; }
    public Guid? ContactId { get; set; }

    // Navigation
    public User AssignedTo { get; set; } = null!;
    public Lead? Lead { get; set; }
    public Deal? Deal { get; set; }
    public Contact? Contact { get; set; }

    public bool IsOverdue => Status != SalesTaskStatus.Completed 
                             && Status != SalesTaskStatus.Cancelled 
                             && DueDate < DateTime.UtcNow;

    public void Complete()
    {
        Status = SalesTaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }
}
