using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;
using SalesAI.Domain.Events;

namespace SalesAI.Domain.Entities;

public class Deal : AuditableEntity
{
    public string Title { get; set; } = null!;
    public decimal Value { get; set; }
    public string Currency { get; set; } = "USD";
    public DealStage Stage { get; set; } = DealStage.NewLead;
    public int Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public string? LostReason { get; set; }

    // Foreign Keys
    public Guid? LeadId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid OwnerId { get; set; }

    // Navigation
    public Lead? Lead { get; set; }
    public Company? Company { get; set; }
    public User Owner { get; set; } = null!;
    public ICollection<DealStageHistory> StageHistory { get; set; } = [];
    public ICollection<SalesTask> Tasks { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<Activity> Activities { get; set; } = [];

    public void MoveToStage(DealStage newStage, string changedBy)
    {
        var previousStage = Stage;
        Stage = newStage;

        // Set probability based on stage
        Probability = newStage switch
        {
            DealStage.NewLead => 10,
            DealStage.Qualified => 25,
            DealStage.Proposal => 50,
            DealStage.Negotiation => 75,
            DealStage.Won => 100,
            DealStage.Lost => 0,
            _ => Probability
        };

        if (newStage == DealStage.Won || newStage == DealStage.Lost)
            ActualCloseDate = DateTime.UtcNow;

        AddDomainEvent(new DealStageChangedEvent(Id, previousStage, newStage, changedBy));
    }
}
