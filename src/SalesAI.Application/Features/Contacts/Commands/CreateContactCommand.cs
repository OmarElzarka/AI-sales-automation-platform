using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Entities;

namespace SalesAI.Application.Features.Contacts.Commands;

public record CreateContactCommand(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? JobTitle,
    bool IsPrimary,
    Guid CompanyId) : IRequest<Result<Guid>>;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(v => v.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.LastName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Email).EmailAddress().MaximumLength(255).When(v => !string.IsNullOrEmpty(v.Email));
        RuleFor(v => v.Phone).MaximumLength(50);
        RuleFor(v => v.JobTitle).MaximumLength(100);
        RuleFor(v => v.CompanyId).NotEmpty();
    }
}

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateContactCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = new Contact
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            JobTitle = request.JobTitle,
            IsPrimary = request.IsPrimary,
            CompanyId = request.CompanyId
        };

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(contact.Id);
    }
}
