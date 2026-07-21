using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAI.Application.Common.Interfaces;

namespace SalesAI.Infrastructure.Services;

public class GeminiAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiAIService> _logger;

    public GeminiAIService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiAIService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<string> GenerateContentAsync(string prompt, CancellationToken ct)
    {
        var apiKey = _configuration["GeminiSettings:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Gemini API key is missing.");
            throw new InvalidOperationException("Gemini API key is not configured.");
        }

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                temperature = 0.2,
                responseMimeType = "application/json"
            }
        };

        var response = await _httpClient.PostAsJsonAsync(url, requestBody, ct);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("Gemini API returned error: {StatusCode} - {Error}", response.StatusCode, error);
            throw new Exception($"Gemini API Error: {response.StatusCode}");
        }

        var jsonString = await response.Content.ReadAsStringAsync(ct);
        var jsonNode = JsonNode.Parse(jsonString);
        var textResult = jsonNode?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();

        if (string.IsNullOrEmpty(textResult))
        {
            throw new Exception("Received empty response from Gemini API.");
        }

        return textResult;
    }

    public async Task<LeadScoreResult> ScoreLeadAsync(LeadScoringContext context, CancellationToken ct = default)
    {
        var prompt = $@"
        You are an AI Sales Assistant. Your task is to score a lead based on the provided context.
        Analyze the following lead and provide a score from 0 to 100, a category (Cold, Warm, Hot), reasoning, and specific factors.
        
        Lead Details:
        Name: {context.FirstName} {context.LastName}
        Email: {context.Email}
        Job Title: {context.JobTitle}
        Company: {context.CompanyName}
        Industry: {context.Industry}
        Employee Count: {context.EmployeeCount}
        Source: {context.Source}
        Status: {context.Status}
        
        Recent Notes:
        {string.Join("\n", context.RecentNotes)}
        
        Recent Activities:
        {string.Join("\n", context.RecentActivities)}

        Return ONLY a JSON object that matches this schema exactly:
        {{
            ""category"": ""Hot|Warm|Cold"",
            ""numericScore"": number,
            ""reasoning"": ""string"",
            ""factors"": [
                {{ ""factor"": ""string"", ""impact"": ""Positive|Negative|Neutral"", ""detail"": ""string"" }}
            ],
            ""recommendedAction"": ""string""
        }}
        ";

        var responseJson = await GenerateContentAsync(prompt, ct);
        return JsonSerializer.Deserialize<LeadScoreResult>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<CompanyResearchResult> ResearchCompanyAsync(CompanyResearchContext context, CancellationToken ct = default)
    {
        var prompt = $@"
        You are an AI Sales Strategist. Research the following company and provide insights to help a sales rep prepare for a call.
        Our Product: {context.OurProductDescription}
        
        Target Company:
        Name: {context.CompanyName}
        Website: {context.Website}
        Description: {context.Description}
        Industry: {context.Industry}

        Return ONLY a JSON object that matches this schema exactly:
        {{
            ""companySummary"": ""string"",
            ""industry"": ""string"",
            ""estimatedSize"": ""string"",
            ""painPoints"": [
                {{ ""pain"": ""string"", ""evidence"": ""string"" }}
            ],
            ""potentialNeeds"": [""string""],
            ""suggestedPitch"": ""string"",
            ""talkingPoints"": [""string""],
            ""competitorsToWatch"": [""string""]
        }}
        ";

        var responseJson = await GenerateContentAsync(prompt, ct);
        return JsonSerializer.Deserialize<CompanyResearchResult>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<GeneratedEmailResult> GenerateEmailAsync(EmailGenerationContext context, CancellationToken ct = default)
    {
        var prompt = $@"
        You are an expert Sales Copywriter. Generate a highly personalized sales email.
        
        Recipient: {context.RecipientFirstName} {context.RecipientLastName}, {context.RecipientJobTitle} at {context.CompanyName}
        Industry: {context.Industry}
        Pain Points: {string.Join(", ", context.PainPoints)}
        Our Product: {context.ProductName} - {context.ProductDescription}
        Tone: {context.Tone}
        Goal: {context.EmailGoal}

        Return ONLY a JSON object that matches this schema exactly:
        {{
            ""subjectLine"": ""string"",
            ""body"": ""string (can contain HTML/markdown)"",
            ""callToAction"": ""string"",
            ""personalizationNotes"": ""string""
        }}
        ";

        var responseJson = await GenerateContentAsync(prompt, ct);
        return JsonSerializer.Deserialize<GeneratedEmailResult>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<MeetingSummaryResult> SummarizeMeetingAsync(MeetingSummaryContext context, CancellationToken ct = default)
    {
        var prompt = $@"
        You are an AI Sales Assistant. Summarize the following meeting transcript/notes and extract actionable insights.
        
        Meeting Date: {context.MeetingDate}
        Attendees: {context.Attendees}
        Deal: {context.DealTitle} (Stage: {context.DealStage})
        Transcript/Notes:
        {context.TranscriptOrNotes}

        Return ONLY a JSON object that matches this schema exactly:
        {{
            ""summary"": ""string"",
            ""keyDiscussionPoints"": [""string""],
            ""actionItems"": [
                {{ ""action"": ""string"", ""owner"": ""string"", ""deadline"": ""string"" }}
            ],
            ""customerObjections"": [
                {{ ""objection"": ""string"", ""suggestedResponse"": ""string"" }}
            ],
            ""risks"": [
                {{ ""riskDescription"": ""string"", ""severity"": ""High|Medium|Low"", ""mitigation"": ""string"" }}
            ],
            ""nextSteps"": [""string""],
            ""sentiment"": ""Positive|Neutral|Negative"",
            ""dealImpact"": ""string""
        }}
        ";

        var responseJson = await GenerateContentAsync(prompt, ct);
        return JsonSerializer.Deserialize<MeetingSummaryResult>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public async Task<SalesPlaybookResult> GeneratePlaybookAsync(PlaybookContext context, CancellationToken ct = default)
    {
        var prompt = $@"
        You are a seasoned AI Sales Director. Provide a step-by-step playbook for how to approach and close this specific lead.
        
        Lead: {context.FirstName} {context.LastName}, {context.JobTitle} at {context.CompanyName}
        Score: {context.NumericScore} ({context.ScoreCategory})
        Status: {context.Status}
        Activities: {string.Join(", ", context.Activities)}
        Notes: {string.Join(", ", context.Notes)}
        Research Context: {context.CompanyResearch}

        Return ONLY a JSON object that matches this schema exactly:
        {{
            ""bestChannel"": {{ ""channel"": ""string"", ""reasoning"": ""string"" }},
            ""bestTimeToContact"": {{ ""dayOfWeek"": ""string"", ""timeRange"": ""string"", ""reasoning"": ""string"" }},
            ""salesApproach"": {{ ""strategy"": ""string"", ""reasoning"": ""string"", ""openingLine"": ""string"" }},
            ""expectedObjections"": [
                {{ ""objection"": ""string"", ""response"": ""string"" }}
            ],
            ""suggestedNextActions"": [
                {{ ""action"": ""string"", ""priority"": ""High|Medium|Low"", ""timing"": ""string"" }}
            ],
            ""competitivePositioning"": ""string""
        }}
        ";

        var responseJson = await GenerateContentAsync(prompt, ct);
        return JsonSerializer.Deserialize<SalesPlaybookResult>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
}
