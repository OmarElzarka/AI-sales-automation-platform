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
        
        // Return mock data if API key is not configured or is the default placeholder
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
        {
            _logger.LogWarning("Gemini API key is not configured. Returning mock AI data for demonstration.");
            return GetMockResponse(prompt);
        }

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}";

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
            
            // Fallback to mock on API error as well to prevent application crashes during demo
            return GetMockResponse(prompt);
        }

        var jsonString = await response.Content.ReadAsStringAsync(ct);
        var jsonNode = JsonNode.Parse(jsonString);
        var partsArray = jsonNode?["candidates"]?[0]?["content"]?["parts"]?.AsArray();
        var textResult = "";
        if (partsArray != null)
        {
            foreach (var part in partsArray)
            {
                textResult += part?["text"]?.GetValue<string>();
            }
        }

        if (string.IsNullOrEmpty(textResult))
        {
            return GetMockResponse(prompt);
        }

        // Clean up markdown formatting if present
        textResult = textResult.Trim();
        if (textResult.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
        {
            textResult = textResult.Substring(7);
        }
        else if (textResult.StartsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            textResult = textResult.Substring(3);
        }

        if (textResult.EndsWith("```", StringComparison.OrdinalIgnoreCase))
        {
            textResult = textResult.Substring(0, textResult.Length - 3);
        }

        textResult = textResult.Trim();
        _logger.LogInformation("Gemini API Returned JSON Payload: {Payload}", textResult);
        return textResult;
    }

    private string GetMockResponse(string prompt)
    {
        if (prompt.Contains("Score a lead", StringComparison.OrdinalIgnoreCase))
        {
            return @"{
                ""category"": ""Hot"",
                ""numericScore"": 92,
                ""reasoning"": ""Strong match with ICP, showed high intent through website activity."",
                ""factors"": [
                    { ""factor"": ""Company Size"", ""impact"": ""Positive"", ""detail"": ""Matches enterprise tier"" }
                ],
                ""recommendedAction"": ""Reach out immediately via phone""
            }";
        }
        else if (prompt.Contains("Research the following company", StringComparison.OrdinalIgnoreCase))
        {
            return @"{
                ""companySummary"": ""A rapidly growing technology company specializing in innovative solutions."",
                ""industry"": ""Technology"",
                ""estimatedSize"": ""500-1000"",
                ""painPoints"": [
                    { ""pain"": ""Scaling infrastructure"", ""evidence"": ""Recent hiring in DevOps roles"" }
                ],
                ""potentialNeeds"": [""Automation tools"", ""Cloud migration support""],
                ""suggestedPitch"": ""Highlight our scalable architecture and automation capabilities."",
                ""talkingPoints"": [""Recent growth"", ""DevOps challenges""],
                ""competitorsToWatch"": [""TechGiant Corp""]
            }";
        }
        else if (prompt.Contains("highly personalized sales email", StringComparison.OrdinalIgnoreCase))
        {
            return @"{
                ""subjectLine"": ""Streamline your operations at [Company]"",
                ""body"": ""Hi there,\n\nI noticed your team is growing rapidly. Our platform can help automate your workflow and save your team 10+ hours a week.\n\nWould you be open to a quick chat?\n\nBest,\nSalesAI"",
                ""callToAction"": ""Book a 10 min meeting"",
                ""personalizationNotes"": ""Referenced their recent growth and focused on time savings.""
            }";
        }
        else if (prompt.Contains("Summarize the following meeting", StringComparison.OrdinalIgnoreCase))
        {
            return @"{
                ""summary"": ""Positive initial discovery call. The prospect is interested but concerned about implementation time."",
                ""keyDiscussionPoints"": [""Current bottlenecks"", ""Pricing"", ""Integration timeline""],
                ""actionItems"": [
                    { ""action"": ""Send technical documentation"", ""owner"": ""Sales Rep"", ""deadline"": ""Tomorrow"" }
                ],
                ""customerObjections"": [
                    { ""objection"": ""Implementation takes too long"", ""suggestedResponse"": ""Share case studies of our 2-week onboarding process."" }
                ],
                ""risks"": [
                    { ""riskDescription"": ""Budget approval required from CFO"", ""severity"": ""Medium"", ""mitigation"": ""Provide ROI calculator."" }
                ],
                ""nextSteps"": [""Follow up next week with technical team""],
                ""sentiment"": ""Positive"",
                ""dealImpact"": ""High - moved to Proposal stage""
            }";
        }
        else if (prompt.Contains("step-by-step playbook", StringComparison.OrdinalIgnoreCase))
        {
            return @"{
                ""bestChannel"": { ""channel"": ""LinkedIn"", ""reasoning"": ""Prospect is highly active on social media."" },
                ""bestTimeToContact"": { ""dayOfWeek"": ""Tuesday"", ""timeRange"": ""Morning"", ""reasoning"": ""Data shows highest engagement for their industry."" },
                ""salesApproach"": { ""strategy"": ""Value-based"", ""reasoning"": ""They are focused on ROI."", ""openingLine"": ""I saw your post about efficiency..."" },
                ""expectedObjections"": [
                    { ""objection"": ""We already use a similar tool"", ""response"": ""Our unique AI scoring sets us apart."" }
                ],
                ""suggestedNextActions"": [
                    { ""action"": ""Connect on LinkedIn"", ""priority"": ""High"", ""timing"": ""Immediately"" }
                ],
                ""competitivePositioning"": ""We are the only platform with built-in Gemini AI.""
            }";
        }
        
        return "{}";
    }

    private T DeserializeWithFallback<T>(string json, string prompt)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Gemini response. Falling back to mock data. Invalid JSON was: {Json}", json);
            var mockJson = GetMockResponse(prompt);
            return JsonSerializer.Deserialize<T>(mockJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
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
        return DeserializeWithFallback<LeadScoreResult>(responseJson, prompt);
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
        return DeserializeWithFallback<CompanyResearchResult>(responseJson, prompt);
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
        return DeserializeWithFallback<GeneratedEmailResult>(responseJson, prompt);
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
        return DeserializeWithFallback<MeetingSummaryResult>(responseJson, prompt);
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
        return DeserializeWithFallback<SalesPlaybookResult>(responseJson, prompt);
    }
}
