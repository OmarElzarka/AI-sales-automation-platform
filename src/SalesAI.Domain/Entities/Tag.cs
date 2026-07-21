using SalesAI.Domain.Common;

namespace SalesAI.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation (many-to-many)
    public ICollection<Lead> Leads { get; set; } = [];
}
