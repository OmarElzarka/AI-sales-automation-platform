using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Entities;

namespace SalesAI.Application.Features.Companies.Commands;

public record CreateCompanyCommand(
    string Name,
    string? Domain,
    string? Industry,
    string? Description,
    int? EmployeeCount,
    string? Website,
    string? Address,
    string? City,
    string? Country) : IRequest<Result<Guid>>;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
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

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = new Company
        {
            Name = request.Name,
            Domain = request.Domain,
            Industry = request.Industry,
            Description = request.Description,
            EmployeeCount = request.EmployeeCount,
            Website = request.Website,
            Address = request.Address,
            City = request.City,
            Country = request.Country
        };

        _context.Companies.Add(company);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(company.Id);
    }
}
