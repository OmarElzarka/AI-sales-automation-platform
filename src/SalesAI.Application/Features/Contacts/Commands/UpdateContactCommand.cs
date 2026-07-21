using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Contacts.Commands;

public record UpdateContactCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? JobTitle,
    bool IsPrimary,
    Guid CompanyId) : IRequest<Result<Guid>>;

public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Email).EmailAddress().MaximumLength(255).When(v => !string.IsNullOrEmpty(v.Email));
        RuleFor(v => v.Phone).MaximumLength(50);
        RuleFor(v => v.JobTitle).MaximumLength(100);
        RuleFor(v => v.CompanyId).NotEmpty();
    }
}

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _context.Contacts.FindAsync(new object[] { request.Id }, cancellationToken);

        if (contact == null)
        {
            return Result<Guid>.Failure("Contact not found.");
        }

        contact.FirstName = request.FirstName;
        contact.LastName = request.LastName;
        contact.Email = request.Email;
        contact.Phone = request.Phone;
        contact.JobTitle = request.JobTitle;
        contact.IsPrimary = request.IsPrimary;
        contact.CompanyId = request.CompanyId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(contact.Id);
    }
}
