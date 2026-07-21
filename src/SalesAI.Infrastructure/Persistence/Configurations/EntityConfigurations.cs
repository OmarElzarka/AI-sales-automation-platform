using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(50);
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Ignore(u => u.FullName);
        builder.Ignore(u => u.DomainEvents);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Token).HasMaxLength(500).IsRequired();
        builder.HasIndex(r => r.Token);
        builder.HasIndex(r => r.UserId);
        builder.Property(r => r.ReplacedByToken).HasMaxLength(500);

        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId);

        builder.Ignore(r => r.DomainEvents);
    }
}

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(300).IsRequired();
        builder.HasIndex(c => c.Name);
        builder.Property(c => c.Domain).HasMaxLength(300);
        builder.HasIndex(c => c.Domain);
        builder.Property(c => c.Industry).HasMaxLength(200);
        builder.Property(c => c.Website).HasMaxLength(500);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.City).HasMaxLength(200);
        builder.Property(c => c.Country).HasMaxLength(200);
        builder.HasQueryFilter(c => !c.IsDeleted);
        builder.Ignore(c => c.DomainEvents);
    }
}

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.LastName).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.HasIndex(c => c.Email);
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.JobTitle).HasMaxLength(200);
        builder.HasIndex(c => c.CompanyId);

        builder.HasOne(c => c.Company)
            .WithMany(co => co.Contacts)
            .HasForeignKey(c => c.CompanyId);

        builder.HasQueryFilter(c => !c.IsDeleted);
        builder.Ignore(c => c.FullName);
        builder.Ignore(c => c.DomainEvents);
    }
}

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(l => l.LastName).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(l => l.Email);
        builder.Property(l => l.Phone).HasMaxLength(50);
        builder.Property(l => l.JobTitle).HasMaxLength(200);
        builder.Property(l => l.LinkedInUrl).HasMaxLength(500);
        builder.Property(l => l.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(l => l.Status);
        builder.Property(l => l.Source).HasConversion<string>().HasMaxLength(50);
        builder.Property(l => l.ScoreCategory).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(l => l.ScoreCategory);
        builder.HasIndex(l => l.AssignedToId);
        builder.HasIndex(l => l.CompanyId);
        builder.HasIndex(l => l.CreatedAt);
        builder.HasIndex(l => l.IsDeleted);

        builder.HasOne(l => l.Company)
            .WithMany(c => c.Leads)
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(l => l.AssignedTo)
            .WithMany(u => u.AssignedLeads)
            .HasForeignKey(l => l.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Tags)
            .WithMany(t => t.Leads)
            .UsingEntity("LeadTags");

        builder.HasQueryFilter(l => !l.IsDeleted);
        builder.Ignore(l => l.FullName);
        builder.Ignore(l => l.DomainEvents);
    }
}

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Title).HasMaxLength(300).IsRequired();
        builder.Property(d => d.Value).HasPrecision(18, 2);
        builder.Property(d => d.Currency).HasMaxLength(3);
        builder.Property(d => d.Stage).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(d => d.Stage);
        builder.Property(d => d.LostReason).HasMaxLength(500);
        builder.HasIndex(d => d.OwnerId);
        builder.HasIndex(d => d.IsDeleted);

        builder.HasOne(d => d.Lead)
            .WithMany()
            .HasForeignKey(d => d.LeadId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Company)
            .WithMany(c => c.Deals)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Owner)
            .WithMany(u => u.OwnedDeals)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(d => !d.IsDeleted);
        builder.Ignore(d => d.DomainEvents);
    }
}

public class DealStageHistoryConfiguration : IEntityTypeConfiguration<DealStageHistory>
{
    public void Configure(EntityTypeBuilder<DealStageHistory> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.FromStage).HasConversion<string>().HasMaxLength(50);
        builder.Property(d => d.ToStage).HasConversion<string>().HasMaxLength(50);
        builder.Property(d => d.ChangedBy).HasMaxLength(256);
        builder.HasIndex(d => d.DealId);

        builder.HasOne(d => d.Deal)
            .WithMany(deal => deal.StageHistory)
            .HasForeignKey(d => d.DealId);

        builder.Ignore(d => d.DomainEvents);
    }
}

public class SalesTaskConfiguration : IEntityTypeConfiguration<SalesTask>
{
    public void Configure(EntityTypeBuilder<SalesTask> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).HasMaxLength(300).IsRequired();
        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(50);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(t => t.AssignedToId);
        builder.HasIndex(t => t.DueDate);
        builder.HasIndex(t => t.Status);

        builder.HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Lead)
            .WithMany(l => l.Tasks)
            .HasForeignKey(t => t.LeadId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Deal)
            .WithMany(d => d.Tasks)
            .HasForeignKey(t => t.DealId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(t => t.IsOverdue);
        builder.Ignore(t => t.DomainEvents);
    }
}

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Title).HasMaxLength(300).IsRequired();
        builder.HasIndex(a => a.LeadId);
        builder.HasIndex(a => a.DealId);
        builder.HasIndex(a => a.ContactId);
        builder.HasIndex(a => a.CreatedAt);

        builder.HasOne(a => a.Lead)
            .WithMany(l => l.Activities)
            .HasForeignKey(a => a.LeadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Deal)
            .WithMany(d => d.Activities)
            .HasForeignKey(a => a.DealId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.DomainEvents);
    }
}

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Content).IsRequired();
        builder.HasIndex(n => n.LeadId);
        builder.HasIndex(n => n.DealId);

        builder.HasOne(n => n.Lead)
            .WithMany(l => l.Notes)
            .HasForeignKey(n => n.LeadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Deal)
            .WithMany(d => d.Notes)
            .HasForeignKey(n => n.DealId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Author)
            .WithMany()
            .HasForeignKey(n => n.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(n => n.DomainEvents);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(t => t.Name).IsUnique();
        builder.Property(t => t.Color).HasMaxLength(7);
        builder.Ignore(t => t.DomainEvents);
    }
}

public class LeadScoreHistoryConfiguration : IEntityTypeConfiguration<LeadScoreHistory>
{
    public void Configure(EntityTypeBuilder<LeadScoreHistory> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Category).HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.Model).HasMaxLength(100);
        builder.HasIndex(l => l.LeadId);

        builder.HasOne(l => l.Lead)
            .WithMany(lead => lead.ScoreHistory)
            .HasForeignKey(l => l.LeadId);

        builder.Ignore(l => l.DomainEvents);
    }
}

public class AIGeneratedContentConfiguration : IEntityTypeConfiguration<AIGeneratedContent>
{
    public void Configure(EntityTypeBuilder<AIGeneratedContent> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(a => a.Type);
        builder.Property(a => a.ContentJson).IsRequired();
        builder.Property(a => a.Model).HasMaxLength(100);
        builder.HasIndex(a => a.LeadId);

        builder.Ignore(a => a.DomainEvents);
    }
}

public class AutomationLogConfiguration : IEntityTypeConfiguration<AutomationLog>
{
    public void Configure(EntityTypeBuilder<AutomationLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.WorkflowName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.TriggerEvent).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Status).HasMaxLength(50);
        builder.HasIndex(a => a.WorkflowName);
        builder.HasIndex(a => a.Status);
        builder.Property(a => a.EntityType).HasMaxLength(100);

        builder.Ignore(a => a.DomainEvents);
    }
}
