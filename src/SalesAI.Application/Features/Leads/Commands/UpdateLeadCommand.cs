using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Enums;
using SalesAI.Domain.ValueObjects;

namespace SalesAI.Application.Features.Leads.Commands;

public record UpdateLeadCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    Guid? CompanyId,
    string? JobTitle,
    string Status,
    string Source,
    Guid AssignedToId) : IRequest<Result<Guid>>;

public class UpdateLeadCommandValidator : AbstractValidator<UpdateLeadCommand>
{
    public UpdateLeadCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(v => v.Phone).MaximumLength(50);
        RuleFor(v => v.JobTitle).MaximumLength(100);
        RuleFor(v => v.Status).NotEmpty().IsEnumName(typeof(LeadStatus), caseSensitive: false);
        RuleFor(v => v.Source).NotEmpty().IsEnumName(typeof(LeadSource), caseSensitive: false);
    }
}

public class UpdateLeadCommandHandler : IRequestHandler<UpdateLeadCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateLeadCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UpdateLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _context.Leads.FindAsync(new object[] { request.Id }, cancellationToken);

        if (lead == null)
        {
            return Result<Guid>.Failure("Lead not found.");
        }

        if (!Enum.TryParse<LeadSource>(request.Source, true, out var leadSource))
        {
            return Result<Guid>.Failure("Invalid lead source.");
        }

        if (!Enum.TryParse<LeadStatus>(request.Status, true, out var leadStatus))
        {
            return Result<Guid>.Failure("Invalid lead status.");
        }

        lead.FirstName = request.FirstName;
        lead.LastName = request.LastName;
        lead.Email = request.Email.ToLowerInvariant();
        lead.Phone = request.Phone;
        lead.CompanyId = request.CompanyId;
        lead.JobTitle = request.JobTitle;
        lead.Source = leadSource;
        lead.Status = leadStatus;
        lead.AssignedToId = request.AssignedToId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(lead.Id);
    }
}
