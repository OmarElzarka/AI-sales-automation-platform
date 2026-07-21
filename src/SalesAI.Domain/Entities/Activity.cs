using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Entities;

public class Activity : BaseEntity
{
    public ActivityType Type { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Guid? PerformedById { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? DealId { get; set; }
    public Guid? ContactId { get; set; }

    // Navigation
    public User? PerformedBy { get; set; }
    public Lead? Lead { get; set; }
    public Deal? Deal { get; set; }
    public Contact? Contact { get; set; }

    public static Activity CreateForLead(Guid leadId, ActivityType type, string title, string? description = null, Guid? userId = null)
    {
        return new Activity
        {
            LeadId = leadId,
            Type = type,
            Title = title,
            Description = description,
            PerformedById = userId
        };
    }

    public static Activity CreateForDeal(Guid dealId, ActivityType type, string title, string? description = null, Guid? userId = null)
    {
        return new Activity
        {
            DealId = dealId,
            Type = type,
            Title = title,
            Description = description,
            PerformedById = userId
        };
    }
}
