using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Entities;

public class ContactSubmission : AuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public ContactInquiryType InquiryType { get; set; } = ContactInquiryType.General;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsProcessed { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
