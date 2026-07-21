using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed admin user if no users exist
        if (!context.Users.Any())
        {
            var adminUser = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Email = "admin@salesai.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                IsActive = true
            };

            var managerUser = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Email = "manager@salesai.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                FirstName = "Sarah",
                LastName = "Johnson",
                Role = UserRole.Manager,
                IsActive = true
            };

            var salesRep = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Email = "rep@salesai.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rep123!"),
                FirstName = "Alex",
                LastName = "Chen",
                Role = UserRole.SalesRep,
                IsActive = true
            };

            context.Users.AddRange(adminUser, managerUser, salesRep);
        }

        // Seed tags
        if (!context.Tags.Any())
        {
            var tags = new List<Tag>
            {
                new() { Name = "Enterprise", Color = "#6366F1" },
                new() { Name = "SMB", Color = "#8B5CF6" },
                new() { Name = "Startup", Color = "#10B981" },
                new() { Name = "High Priority", Color = "#EF4444" },
                new() { Name = "Follow Up", Color = "#F59E0B" },
                new() { Name = "Decision Maker", Color = "#3B82F6" },
                new() { Name = "Technical", Color = "#6B7280" },
                new() { Name = "Referral", Color = "#EC4899" }
            };
            context.Tags.AddRange(tags);
        }

        // Seed sample companies
        if (!context.Companies.Any())
        {
            var companies = new List<Company>
            {
                new()
                {
                    Name = "TechCorp Solutions",
                    Domain = "techcorp.com",
                    Industry = "Technology",
                    Description = "Enterprise software solutions provider",
                    EmployeeCount = 500,
                    Website = "https://techcorp.com",
                    City = "San Francisco",
                    Country = "USA"
                },
                new()
                {
                    Name = "GreenEnergy Inc",
                    Domain = "greenenergy.com",
                    Industry = "Renewable Energy",
                    Description = "Solar and wind energy solutions",
                    EmployeeCount = 200,
                    Website = "https://greenenergy.com",
                    City = "Austin",
                    Country = "USA"
                },
                new()
                {
                    Name = "FinanceFlow",
                    Domain = "financeflow.io",
                    Industry = "Financial Services",
                    Description = "AI-powered financial analytics platform",
                    EmployeeCount = 150,
                    Website = "https://financeflow.io",
                    City = "New York",
                    Country = "USA"
                },
                new()
                {
                    Name = "HealthPlus Medical",
                    Domain = "healthplus.com",
                    Industry = "Healthcare",
                    Description = "Digital health solutions for hospitals",
                    EmployeeCount = 1000,
                    Website = "https://healthplus.com",
                    City = "Boston",
                    Country = "USA"
                },
                new()
                {
                    Name = "EduLearn Platform",
                    Domain = "edulearn.com",
                    Industry = "Education Technology",
                    Description = "Online learning and certification platform",
                    EmployeeCount = 80,
                    Website = "https://edulearn.com",
                    City = "London",
                    Country = "UK"
                }
            };
            context.Companies.AddRange(companies);
            await context.SaveChangesAsync();

            // Seed leads with company associations
            var repId = Guid.Parse("00000000-0000-0000-0000-000000000003");
            var managerId = Guid.Parse("00000000-0000-0000-0000-000000000002");

            var leads = new List<Lead>
            {
                new()
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@techcorp.com",
                    JobTitle = "CTO",
                    Phone = "+1-555-0101",
                    Status = LeadStatus.Qualified,
                    Source = LeadSource.LinkedIn,
                    ScoreCategory = LeadScoreCategory.Hot,
                    ScoreNumeric = 92,
                    ScoreReasoning = "C-level executive at mid-size tech company, sourced via LinkedIn — high conversion potential.",
                    CompanyId = companies[0].Id,
                    AssignedToId = repId
                },
                new()
                {
                    FirstName = "Emily",
                    LastName = "Davis",
                    Email = "emily.davis@greenenergy.com",
                    JobTitle = "VP of Operations",
                    Phone = "+1-555-0102",
                    Status = LeadStatus.Contacted,
                    Source = LeadSource.Website,
                    ScoreCategory = LeadScoreCategory.Warm,
                    ScoreNumeric = 68,
                    ScoreReasoning = "VP-level, growing industry — moderate fit but engagement signals are positive.",
                    CompanyId = companies[1].Id,
                    AssignedToId = repId
                },
                new()
                {
                    FirstName = "Michael",
                    LastName = "Chen",
                    Email = "m.chen@financeflow.io",
                    JobTitle = "Head of Engineering",
                    Phone = "+1-555-0103",
                    Status = LeadStatus.New,
                    Source = LeadSource.Referral,
                    CompanyId = companies[2].Id,
                    AssignedToId = managerId
                },
                new()
                {
                    FirstName = "Lisa",
                    LastName = "Brown",
                    Email = "lisa.brown@healthplus.com",
                    JobTitle = "Director of Digital Transformation",
                    Status = LeadStatus.Qualified,
                    Source = LeadSource.Event,
                    ScoreCategory = LeadScoreCategory.Hot,
                    ScoreNumeric = 88,
                    ScoreReasoning = "Director at large healthcare org, met at industry event — strong buying signals.",
                    CompanyId = companies[3].Id,
                    AssignedToId = repId
                },
                new()
                {
                    FirstName = "David",
                    LastName = "Wilson",
                    Email = "d.wilson@edulearn.com",
                    JobTitle = "Product Manager",
                    Status = LeadStatus.New,
                    Source = LeadSource.ColdOutreach,
                    ScoreCategory = LeadScoreCategory.Cold,
                    ScoreNumeric = 35,
                    ScoreReasoning = "Mid-level role at small EdTech — limited budget authority.",
                    CompanyId = companies[4].Id,
                    AssignedToId = managerId
                }
            };
            context.Leads.AddRange(leads);
            await context.SaveChangesAsync();

            // Seed some deals
            var deals = new List<Deal>
            {
                new()
                {
                    Title = "TechCorp Enterprise License",
                    Value = 75000,
                    Stage = DealStage.Proposal,
                    Probability = 50,
                    ExpectedCloseDate = DateTime.UtcNow.AddDays(30),
                    LeadId = leads[0].Id,
                    CompanyId = companies[0].Id,
                    OwnerId = repId
                },
                new()
                {
                    Title = "GreenEnergy Operations Suite",
                    Value = 45000,
                    Stage = DealStage.Qualified,
                    Probability = 25,
                    ExpectedCloseDate = DateTime.UtcNow.AddDays(60),
                    LeadId = leads[1].Id,
                    CompanyId = companies[1].Id,
                    OwnerId = repId
                },
                new()
                {
                    Title = "HealthPlus Digital Platform",
                    Value = 120000,
                    Stage = DealStage.Negotiation,
                    Probability = 75,
                    ExpectedCloseDate = DateTime.UtcNow.AddDays(14),
                    LeadId = leads[3].Id,
                    CompanyId = companies[3].Id,
                    OwnerId = repId
                },
                new()
                {
                    Title = "FinanceFlow Analytics Integration",
                    Value = 30000,
                    Stage = DealStage.NewLead,
                    Probability = 10,
                    ExpectedCloseDate = DateTime.UtcNow.AddDays(90),
                    LeadId = leads[2].Id,
                    CompanyId = companies[2].Id,
                    OwnerId = managerId
                }
            };
            context.Deals.AddRange(deals);

            // Seed tasks
            var tasks = new List<SalesTask>
            {
                new()
                {
                    Title = "Follow up with John Smith on proposal",
                    Type = TaskType.FollowUp,
                    Priority = TaskPriority.High,
                    DueDate = DateTime.UtcNow.AddDays(2),
                    AssignedToId = repId,
                    LeadId = leads[0].Id,
                    DealId = deals[0].Id
                },
                new()
                {
                    Title = "Schedule demo with Emily Davis",
                    Type = TaskType.Demo,
                    Priority = TaskPriority.Medium,
                    DueDate = DateTime.UtcNow.AddDays(5),
                    AssignedToId = repId,
                    LeadId = leads[1].Id
                },
                new()
                {
                    Title = "Call Lisa Brown - contract negotiation",
                    Type = TaskType.Call,
                    Priority = TaskPriority.Urgent,
                    DueDate = DateTime.UtcNow.AddDays(1),
                    AssignedToId = repId,
                    LeadId = leads[3].Id,
                    DealId = deals[2].Id
                }
            };
            context.SalesTasks.AddRange(tasks);

            // Seed activities
            var activities = new List<Activity>
            {
                Activity.CreateForLead(leads[0].Id, ActivityType.LeadScored, "Lead scored as Hot (92/100)", "C-level executive at mid-size tech company"),
                Activity.CreateForLead(leads[0].Id, ActivityType.Email, "Sent initial outreach email", null, repId),
                Activity.CreateForLead(leads[0].Id, ActivityType.Call, "Discovery call completed", "Discussed needs around enterprise deployment", repId),
                Activity.CreateForDeal(deals[0].Id, ActivityType.StageChange, "Deal moved to Proposal", null, repId),
                Activity.CreateForLead(leads[1].Id, ActivityType.LeadScored, "Lead scored as Warm (68/100)", "VP-level, growing industry"),
                Activity.CreateForLead(leads[3].Id, ActivityType.Meeting, "Product demo completed", "Showed healthcare compliance features", repId),
                Activity.CreateForDeal(deals[2].Id, ActivityType.StageChange, "Deal moved to Negotiation", null, repId),
            };
            context.Activities.AddRange(activities);
        }

        await context.SaveChangesAsync();
    }
}
