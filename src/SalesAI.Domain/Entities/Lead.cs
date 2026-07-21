using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;
using SalesAI.Domain.Events;

namespace SalesAI.Domain.Entities;

public class Lead : AuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? LinkedInUrl { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public LeadSource Source { get; set; } = LeadSource.Other;

    // AI Score (denormalized for quick access)
    public LeadScoreCategory? ScoreCategory { get; set; }
    public int? ScoreNumeric { get; set; }
    public string? ScoreReasoning { get; set; }

    // Foreign Keys
    public Guid? CompanyId { get; set; }
    public Guid AssignedToId { get; set; }
    public Guid? ContactId { get; set; }

    // Navigation
    public Company? Company { get; set; }
    public User AssignedTo { get; set; } = null!;
    public Contact? Contact { get; set; }
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<Activity> Activities { get; set; } = [];
    public ICollection<SalesTask> Tasks { get; set; } = [];
    public ICollection<LeadScoreHistory> ScoreHistory { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateScore(LeadScoreCategory category, int numericScore, string reasoning)
    {
        ScoreCategory = category;
        ScoreNumeric = numericScore;
        ScoreReasoning = reasoning;
        AddDomainEvent(new LeadScoredEvent(Id, category, numericScore));
    }

    public static Lead Create(string firstName, string lastName, string email, LeadSource source, Guid assignedToId)
    {
        var lead = new Lead
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email.ToLowerInvariant(),
            Source = source,
            Status = LeadStatus.New,
            AssignedToId = assignedToId
        };

        lead.AddDomainEvent(new LeadCreatedEvent(lead.Id));
        return lead;
    }
}
