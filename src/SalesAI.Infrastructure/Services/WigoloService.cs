using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAI.Application.Common.Interfaces;

namespace SalesAI.Infrastructure.Services;

public class WigoloService : IWigoloService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WigoloService> _logger;

    public WigoloService(HttpClient httpClient, IConfiguration configuration, ILogger<WigoloService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        var baseUrl = configuration["WigoloApi:BaseUrl"] ?? "http://wigolo:3333";
        _httpClient.BaseAddress = new Uri(baseUrl);
        
        var token = configuration["WigoloApi:Token"] ?? "WigoloSecret2026";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<string> ResearchCompanyAsync(string companyName, string? website, CancellationToken ct = default)
    {
        var query = $"Provide a comprehensive business overview of {companyName}";
        if (!string.IsNullOrEmpty(website))
        {
            query += $" (website: {website}). Include their products, industry, competitors, pricing, target customers, market positioning, and recent news.";
        }

        return await ExecuteResearchToolAsync(query, ct);
    }

    public async Task<string> ResearchLeadAsync(string firstName, string lastName, string companyName, string? jobTitle, CancellationToken ct = default)
    {
        var name = $"{firstName} {lastName}";
        var query = $"Gather publicly available professional information about {name}, {jobTitle} at {companyName}. Include professional biography, achievements, and LinkedIn-style profile information.";
        
        return await ExecuteResearchToolAsync(query, ct);
    }

    private async Task<string> ExecuteResearchToolAsync(string query, CancellationToken ct)
    {
        try
        {
            var requestBody = new { question = query };
            var response = await _httpClient.PostAsJsonAsync("/v1/research", requestBody, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Wigolo API failed with {StatusCode}: {Error}. Falling back to empty object.", response.StatusCode, error);
                return "{}";
            }

            var result = await response.Content.ReadAsStringAsync(ct);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error communicating with Wigolo API. Returning empty JSON.");
            return "{}";
        }
    }
}
