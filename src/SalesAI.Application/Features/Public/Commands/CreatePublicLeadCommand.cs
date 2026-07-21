using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Public.Models;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Public.Commands;

public record CreatePublicLeadCommand(
    string FirstName,
    string LastName,
    string Company,
    string BusinessEmail,
    string? Phone,
    string? Industry,
    string? CompanySize,
    string? Country,
    string? InterestedProduct,
    string? EstimatedBudget,
    string? Message) : IRequest<Result<PublicLeadResponse>>;

public class CreatePublicLeadCommandValidator : AbstractValidator<CreatePublicLeadCommand>
{
    public CreatePublicLeadCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Company).NotEmpty().MaximumLength(200);
        RuleFor(v => v.BusinessEmail).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(v => v.Phone).MaximumLength(50);
        RuleFor(v => v.Industry).MaximumLength(100);
        RuleFor(v => v.CompanySize).MaximumLength(50);
        RuleFor(v => v.Country).MaximumLength(100);
        RuleFor(v => v.InterestedProduct).MaximumLength(200);
        RuleFor(v => v.EstimatedBudget).MaximumLength(100);
        RuleFor(v => v.Message).MaximumLength(2000);
    }
}

public class CreatePublicLeadCommandHandler : IRequestHandler<CreatePublicLeadCommand, Result<PublicLeadResponse>>
{
    private readonly IApplicationDbContext _context;

    public CreatePublicLeadCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PublicLeadResponse>> Handle(CreatePublicLeadCommand request, CancellationToken cancellationToken)
    {
        // Find or create the company
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Name.ToLower() == request.Company.ToLower(), cancellationToken);

        if (company == null)
        {
            company = new Company
            {
                Name = request.Company,
                Industry = request.Industry,
                Country = request.Country,
                Website = ExtractDomainFromEmail(request.BusinessEmail)
            };

            if (!string.IsNullOrEmpty(request.CompanySize) && Enum.TryParse<CompanySize>(request.CompanySize, true, out var sizeEnum))
            {
                company.EmployeeCount = sizeEnum switch
                {
                    CompanySize.Startup => 10,
                    CompanySize.Small => 50,
                    CompanySize.Medium => 250,
                    CompanySize.Enterprise => 1000,
                    _ => null
                };
            }

            _context.Companies.Add(company);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Round-robin assignment: pick the sales rep with the fewest leads
        var assignedUser = await _context.Users
            .Where(u => u.IsActive && u.Role == UserRole.SalesRep)
            .OrderBy(u => u.AssignedLeads.Count)
            .FirstOrDefaultAsync(cancellationToken);

        // Fallback to any active user if no sales rep exists
        assignedUser ??= await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.AssignedLeads.Count)
            .FirstOrDefaultAsync(cancellationToken);

        if (assignedUser == null)
            return Result<PublicLeadResponse>.Failure("Unable to process request at this time.");

        // Check for duplicate email
        var existingLead = await _context.Leads
            .FirstOrDefaultAsync(l => l.Email == request.BusinessEmail.ToLowerInvariant(), cancellationToken);

        if (existingLead != null)
        {
            return Result<PublicLeadResponse>.Success(
                new PublicLeadResponse(existingLead.Id, "Thank you! We already have your information and will be in touch soon.", "Received"));
        }

        // Create the lead using factory method (triggers domain events)
        var lead = Lead.Create(request.FirstName, request.LastName, request.BusinessEmail, LeadSource.Website, assignedUser.Id);
        lead.Phone = request.Phone;
        lead.CompanyId = company.Id;
        lead.JobTitle = request.InterestedProduct;

        _context.Leads.Add(lead);

        // Add a note with all the public form data
        var noteContent = BuildLeadNote(request);
        var note = new Note
        {
            LeadId = lead.Id,
            Content = noteContent,
            AuthorId = assignedUser.Id
        };
        _context.Notes.Add(note);

        // Log the activity
        _context.Activities.Add(Activity.CreateForLead(
            lead.Id,
            ActivityType.Note,
            "Website Demo Request Submitted",
            $"New demo request from {request.FirstName} {request.LastName} at {request.Company}"));

        await _context.SaveChangesAsync(cancellationToken);

        return Result<PublicLeadResponse>.Success(
            new PublicLeadResponse(lead.Id, "Thank you for your interest! Our team will contact you shortly.", "Received"));
    }

    private static string? ExtractDomainFromEmail(string email)
    {
        var parts = email.Split('@');
        return parts.Length == 2 ? $"https://www.{parts[1]}" : null;
    }

    private static string BuildLeadNote(CreatePublicLeadCommand request)
    {
        var lines = new List<string>
        {
            "=== Website Demo Request ===",
            $"Company: {request.Company}",
            $"Email: {request.BusinessEmail}"
        };

        if (!string.IsNullOrEmpty(request.Phone)) lines.Add($"Phone: {request.Phone}");
        if (!string.IsNullOrEmpty(request.Industry)) lines.Add($"Industry: {request.Industry}");
        if (!string.IsNullOrEmpty(request.CompanySize)) lines.Add($"Company Size: {request.CompanySize}");
        if (!string.IsNullOrEmpty(request.Country)) lines.Add($"Country: {request.Country}");
        if (!string.IsNullOrEmpty(request.InterestedProduct)) lines.Add($"Interested Product: {request.InterestedProduct}");
        if (!string.IsNullOrEmpty(request.EstimatedBudget)) lines.Add($"Estimated Budget: {request.EstimatedBudget}");
        if (!string.IsNullOrEmpty(request.Message)) lines.Add($"\nMessage:\n{request.Message}");

        return string.Join("\n", lines);
    }
}
