namespace SalesAI.Domain.Enums;

public enum LeadStatus
{
    New,
    Contacted,
    Qualified,
    Unqualified,
    Converted,
    Lost
}

public enum LeadSource
{
    Website,
    Referral,
    LinkedIn,
    ColdOutreach,
    Event,
    Advertisement,
    Other
}

public enum LeadScoreCategory
{
    Cold,
    Warm,
    Hot
}

public enum DealStage
{
    NewLead,
    Qualified,
    Proposal,
    Negotiation,
    Won,
    Lost
}

public enum TaskType
{
    Call,
    Meeting,
    FollowUp,
    Email,
    Demo,
    Other
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Urgent
}

public enum SalesTaskStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public enum ActivityType
{
    Note,
    Call,
    Email,
    Meeting,
    StageChange,
    AIAction,
    TaskCompleted,
    LeadScored,
    Notification
}

public enum UserRole
{
    Admin,
    Manager,
    SalesRep
}

public enum AIContentType
{
    CompanyResearch,
    Email,
    MeetingSummary,
    Playbook,
    LeadScore
}

public enum ContactInquiryType
{
    General,
    Sales,
    Support
}

public enum CompanySize
{
    Startup,
    Small,
    Medium,
    Enterprise
}
