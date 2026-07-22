using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Features.Leads.Models;

namespace SalesAI.Application.Features.Leads.Services;

public interface ILeadDuplicateDetectionService
{
    Task<List<LeadDto>> FindPotentialDuplicatesAsync(string email, string? phone, string firstName, string lastName, CancellationToken cancellationToken);
}

public class LeadDuplicateDetectionService : ILeadDuplicateDetectionService
{
    private readonly IApplicationDbContext _context;

    public LeadDuplicateDetectionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<LeadDto>> FindPotentialDuplicatesAsync(string email, string? phone, string firstName, string lastName, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.ToLowerInvariant();
        var normalizedFirstName = firstName.ToLowerInvariant();
        var normalizedLastName = lastName.ToLowerInvariant();

        var query = _context.Leads.Include(l => l.Company).AsNoTracking().AsQueryable();

        var duplicates = await query
            .Where(l => 
                l.Email.ToLower() == normalizedEmail || 
                (phone != null && l.Phone != null && l.Phone == phone) ||
                (l.FirstName.ToLower() == normalizedFirstName && l.LastName.ToLower() == normalizedLastName))
            .ToListAsync(cancellationToken);

        return duplicates.Select(lead => new LeadDto(
                lead.Id,
                lead.FirstName,
                lead.LastName,
                lead.Email,
                lead.Phone,
                lead.Company != null ? lead.Company.Name : null,
                lead.JobTitle,
                lead.Status.ToString(),
                lead.Source.ToString(),
                lead.AssignedToId,
                lead.CompanyId,
                lead.ScoreNumeric,
                lead.CreatedAt,
                lead.ModifiedAt,
                lead.ResearchStatus.ToString())).ToList();
    }
}
