using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;
using SalesAI.Domain.ValueObjects;

namespace SalesAI.Application.Features.Leads.Commands;

public record CreateLeadCommand(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    Guid? CompanyId,
    string? JobTitle,
    string Source,
    Guid AssignedToId) : IRequest<Result<Guid>>;

public class CreateLeadCommandValidator : AbstractValidator<CreateLeadCommand>
{
    public CreateLeadCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(v => v.Phone).MaximumLength(50);
        RuleFor(v => v.JobTitle).MaximumLength(100);
        RuleFor(v => v.Source).NotEmpty().IsEnumName(typeof(LeadSource), caseSensitive: false);
    }
}

public class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateLeadCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<LeadSource>(request.Source, true, out var leadSource))
        {
            return Result<Guid>.Failure("Invalid lead source.");
        }

        var lead = new Lead
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLowerInvariant(),
            Phone = request.Phone,
            CompanyId = request.CompanyId,
            JobTitle = request.JobTitle,
            Source = leadSource,
            Status = LeadStatus.New,
            AssignedToId = request.AssignedToId,
            ScoreNumeric = 0
        };

        _context.Leads.Add(lead);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(lead.Id);
    }
}
