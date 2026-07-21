using SalesAI.Domain.Common;

namespace SalesAI.Domain.Entities;

public class NewsletterSubscription : BaseEntity
{
    public string Email { get; set; } = null!;
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
