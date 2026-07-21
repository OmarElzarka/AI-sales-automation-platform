using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Entities;

public class DealStageHistory : BaseEntity
{
    public Guid DealId { get; set; }
    public DealStage? FromStage { get; set; }
    public DealStage ToStage { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? ChangedBy { get; set; }

    // Navigation
    public Deal Deal { get; set; } = null!;
}
