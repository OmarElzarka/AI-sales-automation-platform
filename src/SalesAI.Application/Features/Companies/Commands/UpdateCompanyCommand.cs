using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Companies.Commands;

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string? Domain,
    string? Industry,
    string? Description,
    int? EmployeeCount,
    string? Website,
    string? Address,
    string? City,
    string? Country) : IRequest<Result<Guid>>;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Name).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Domain).MaximumLength(100);
        RuleFor(v => v.Industry).MaximumLength(100);
        RuleFor(v => v.Description).MaximumLength(2000);
        RuleFor(v => v.Website).MaximumLength(255);
        RuleFor(v => v.Address).MaximumLength(500);
        RuleFor(v => v.City).MaximumLength(100);
        RuleFor(v => v.Country).MaximumLength(100);
    }
}

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(new object[] { request.Id }, cancellationToken);

        if (company == null)
        {
            return Result<Guid>.Failure("Company not found.");
        }

        company.Name = request.Name;
        company.Domain = request.Domain;
        company.Industry = request.Industry;
        company.Description = request.Description;
        company.EmployeeCount = request.EmployeeCount;
        company.Website = request.Website;
        company.Address = request.Address;
        company.City = request.City;
        company.Country = request.Country;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(company.Id);
    }
}
