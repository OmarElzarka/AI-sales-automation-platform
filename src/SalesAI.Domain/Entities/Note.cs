using SalesAI.Domain.Common;

namespace SalesAI.Domain.Entities;

public class Note : BaseEntity
{
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }

    // Foreign Keys
    public Guid? LeadId { get; set; }
    public Guid? DealId { get; set; }
    public Guid AuthorId { get; set; }

    // Navigation
    public Lead? Lead { get; set; }
    public Deal? Deal { get; set; }
    public User Author { get; set; } = null!;
}
