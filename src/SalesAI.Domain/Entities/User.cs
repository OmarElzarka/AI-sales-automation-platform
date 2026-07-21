using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.SalesRep;
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Lead> AssignedLeads { get; set; } = [];
    public ICollection<Deal> OwnedDeals { get; set; } = [];
    public ICollection<SalesTask> AssignedTasks { get; set; } = [];
}
