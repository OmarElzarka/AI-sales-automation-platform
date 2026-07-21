using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Entities;

public class AIGeneratedContent : BaseEntity
{
    public AIContentType Type { get; set; }
    public string ContentJson { get; set; } = null!;
    public int? PromptTokens { get; set; }
    public int? CompletionTokens { get; set; }
    public string? Model { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public Guid? LeadId { get; set; }
    public Guid? DealId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid RequestedById { get; set; }

    // Navigation
    public Lead? Lead { get; set; }
    public Deal? Deal { get; set; }
    public Company? Company { get; set; }
    public User RequestedBy { get; set; } = null!;
}
