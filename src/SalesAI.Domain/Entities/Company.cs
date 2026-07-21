using SalesAI.Domain.Common;

namespace SalesAI.Domain.Entities;

public class Company : AuditableEntity
{
    public string Name { get; set; } = null!;
    public string? Domain { get; set; }
    public string? Industry { get; set; }
    public string? Description { get; set; }
    public int? EmployeeCount { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    // Navigation
    public ICollection<Contact> Contacts { get; set; } = [];
    public ICollection<Lead> Leads { get; set; } = [];
    public ICollection<Deal> Deals { get; set; } = [];
}
