namespace SalesAI.Application.Common.Interfaces;

public interface IAIService
{
    Task<LeadScoreResult> ScoreLeadAsync(LeadScoringContext context, CancellationToken ct = default);
    Task<CompanyResearchResult> ResearchCompanyAsync(CompanyResearchContext context, CancellationToken ct = default);
    Task<GeneratedEmailResult> GenerateEmailAsync(EmailGenerationContext context, CancellationToken ct = default);
    Task<MeetingSummaryResult> SummarizeMeetingAsync(MeetingSummaryContext context, CancellationToken ct = default);
    Task<SalesPlaybookResult> GeneratePlaybookAsync(PlaybookContext context, CancellationToken ct = default);
    Task<CompetitiveIntelligenceResult> GenerateCompetitiveIntelligenceAsync(CompetitiveIntelligenceContext context, CancellationToken ct = default);
}

// === Context Models (Input) ===

public record LeadScoringContext(
    string FirstName,
    string LastName,
    string Email,
    string? JobTitle,
    string? CompanyName,
    string? Industry,
    int? EmployeeCount,
    string Source,
    string Status,
    DateTime CreatedAt,
    DateTime? LastActivityDate,
    List<string> RecentNotes,
    List<string> RecentActivities);

public record CompanyResearchContext(
    string CompanyName,
    string? Website,
    string? Description,
    string? Industry,
    string? OurProductDescription);

public record EmailGenerationContext(
    string RecipientFirstName,
    string RecipientLastName,
    string? RecipientJobTitle,
    string CompanyName,
    string? Industry,
    List<string> PainPoints,
    string? ProductName,
    string? ProductDescription,
    string Tone,
    string? EmailGoal);

public record MeetingSummaryContext(
    string? MeetingDate,
    string? Attendees,
    string? DealTitle,
    string? DealStage,
    decimal? DealValue,
    string TranscriptOrNotes);

public record PlaybookContext(
    string FirstName,
    string LastName,
    string? JobTitle,
    string CompanyName,
    string? Industry,
    int? EmployeeCount,
    string? ScoreCategory,
    int? NumericScore,
    string Status,
    string Source,
    List<string> Activities,
    List<string> Notes,
    string? CompanyResearch);

// === Result Models (Output) ===

public record LeadScoreResult(
    string Category,
    int NumericScore,
    string Reasoning,
    List<ScoreFactor> Factors,
    string RecommendedAction);

public record ScoreFactor(string Factor, string Impact, string Detail);

public record CompanyResearchResult(
    string CompanySummary,
    string Industry,
    string EstimatedSize,
    List<PainPoint> PainPoints,
    List<string> PotentialNeeds,
    string SuggestedPitch,
    List<string> TalkingPoints,
    List<string> CompetitorsToWatch);

public record PainPoint(string Pain, string Evidence);

public record GeneratedEmailResult(
    string SubjectLine,
    string Body,
    string CallToAction,
    string PersonalizationNotes);

public record MeetingSummaryResult(
    string Summary,
    List<string> KeyDiscussionPoints,
    List<ActionItem> ActionItems,
    List<CustomerObjection> CustomerObjections,
    List<Risk> Risks,
    List<string> NextSteps,
    string Sentiment,
    string DealImpact);

public record ActionItem(string Action, string Owner, string Deadline);
public record CustomerObjection(string Objection, string SuggestedResponse);
public record Risk(string RiskDescription, string Severity, string Mitigation);

public record SalesPlaybookResult(
    ChannelRecommendation BestChannel,
    TimingRecommendation BestTimeToContact,
    ApproachRecommendation SalesApproach,
    List<ObjectionResponse> ExpectedObjections,
    List<SuggestedAction> SuggestedNextActions,
    string CompetitivePositioning);

public record ChannelRecommendation(string Channel, string Reasoning);
public record TimingRecommendation(string DayOfWeek, string TimeRange, string Reasoning);
public record ApproachRecommendation(string Strategy, string Reasoning, string OpeningLine);
public record ObjectionResponse(string Objection, string Response);
public record SuggestedAction(string Action, string Priority, string Timing);

public record CompetitiveIntelligenceContext(
    string WigoloCompanyResearch,
    string WigoloLeadResearch);

public record CompetitiveIntelligenceResult(
    string CompanySummary,
    string LeadSummary,
    int LeadScore,
    string LeadTemperature,
    string LeadQualification,
    List<string> BuyingSignals,
    List<string> PotentialPainPoints,
    string RecommendedSalesStrategy,
    List<string> PersonalizedTalkingPoints,
    List<string> SuggestedIceBreakers,
    List<string> RecommendedProductFeaturesToHighlight,
    List<string> PotentialObjections,
    string ExecutiveSummary,
    List<string> RecommendedFollowUpActions);
