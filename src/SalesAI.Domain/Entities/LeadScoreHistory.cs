using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Entities;

public class LeadScoreHistory : BaseEntity
{
    public Guid LeadId { get; set; }
    public LeadScoreCategory Category { get; set; }
    public int NumericScore { get; set; }
    public string? Reasoning { get; set; }
    public string? FactorsJson { get; set; }
    public DateTime ScoredAt { get; set; } = DateTime.UtcNow;
    public string? Model { get; set; }

    // Navigation
    public Lead Lead { get; set; } = null!;
}
