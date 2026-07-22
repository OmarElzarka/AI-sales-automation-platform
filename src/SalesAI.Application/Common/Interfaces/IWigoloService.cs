namespace SalesAI.Application.Common.Interfaces;

public interface IWigoloService
{
    Task<string> ResearchCompanyAsync(string companyName, string? website, CancellationToken ct = default);
    Task<string> ResearchLeadAsync(string firstName, string lastName, string companyName, string? jobTitle, CancellationToken ct = default);
}
