using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Leads.Services;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Leads.Commands;

public record ImportLeadsCommand(byte[] CsvContent, Guid AssignedToId) : IRequest<Result<ImportLeadsResult>>;

public record ImportLeadsResult(int ImportedCount, int SkippedCount, List<string> Errors);

public class CsvLeadRecord
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? Company { get; set; }
}

public class ImportLeadsCommandHandler : IRequestHandler<ImportLeadsCommand, Result<ImportLeadsResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILeadDuplicateDetectionService _duplicateDetectionService;

    public ImportLeadsCommandHandler(
        IApplicationDbContext context, 
        ILeadDuplicateDetectionService duplicateDetectionService)
    {
        _context = context;
        _duplicateDetectionService = duplicateDetectionService;
    }

    public async Task<Result<ImportLeadsResult>> Handle(ImportLeadsCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        int importedCount = 0;
        int skippedCount = 0;

        try
        {
            using var stream = new MemoryStream(request.CsvContent);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            var records = csv.GetRecords<CsvLeadRecord>().ToList();

            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.Email) || string.IsNullOrWhiteSpace(record.FirstName))
                {
                    errors.Add($"Skipped row: Missing essential fields (FirstName or Email). Email: {record.Email}");
                    skippedCount++;
                    continue;
                }

                // Check for duplicates
                var potentialDuplicates = await _duplicateDetectionService.FindPotentialDuplicatesAsync(
                    record.Email, record.Phone, record.FirstName, record.LastName, cancellationToken);

                if (potentialDuplicates.Any())
                {
                    errors.Add($"Skipped duplicate: {record.Email}");
                    skippedCount++;
                    continue;
                }

                // Find or create company if provided
                Guid? companyId = null;
                if (!string.IsNullOrWhiteSpace(record.Company))
                {
                    var company = _context.Companies.FirstOrDefault(c => c.Name.ToLower() == record.Company.ToLower());
                    if (company == null)
                    {
                        company = new Company { Name = record.Company };
                        _context.Companies.Add(company);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    companyId = company.Id;
                }

                var lead = new Lead
                {
                    FirstName = record.FirstName,
                    LastName = record.LastName,
                    Email = record.Email.ToLowerInvariant(),
                    Phone = record.Phone,
                    JobTitle = record.JobTitle,
                    CompanyId = companyId,
                    Source = LeadSource.Other,
                    Status = LeadStatus.New,
                    AssignedToId = request.AssignedToId,
                    ScoreNumeric = 0
                };

                _context.Leads.Add(lead);
                importedCount++;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result<ImportLeadsResult>.Success(new ImportLeadsResult(importedCount, skippedCount, errors));
        }
        catch (Exception ex)
        {
            return Result<ImportLeadsResult>.Failure($"Failed to parse CSV: {ex.Message}");
        }
    }
}
