using SalesAI.Domain.Common;

namespace SalesAI.Domain.Entities;

public class Contact : AuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public bool IsPrimary { get; set; }

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    // Navigation
    public ICollection<Activity> Activities { get; set; } = [];
}
