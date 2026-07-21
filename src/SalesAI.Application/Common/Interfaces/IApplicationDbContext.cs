using Microsoft.EntityFrameworkCore;
using SalesAI.Domain.Entities;

namespace SalesAI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Lead> Leads { get; }
    DbSet<Company> Companies { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Deal> Deals { get; }
    DbSet<DealStageHistory> DealStageHistory { get; }
    DbSet<SalesTask> SalesTasks { get; }
    DbSet<Activity> Activities { get; }
    DbSet<Note> Notes { get; }
    DbSet<Tag> Tags { get; }
    DbSet<LeadScoreHistory> LeadScoreHistory { get; }
    DbSet<AIGeneratedContent> AIGeneratedContent { get; }
    DbSet<AutomationLog> AutomationLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
