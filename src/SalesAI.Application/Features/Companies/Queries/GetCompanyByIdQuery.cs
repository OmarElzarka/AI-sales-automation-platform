using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Companies.Models;

namespace SalesAI.Application.Features.Companies.Queries;

public record GetCompanyByIdQuery(Guid Id) : IRequest<Result<CompanyDto>>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCompanyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            return Result<CompanyDto>.Failure("Company not found.");
        }

        var dto = new CompanyDto(
            company.Id,
            company.Name,
            company.Domain,
            company.Industry,
            company.Description,
            company.EmployeeCount,
            company.Website,
            company.Address,
            company.City,
            company.Country,
            company.CreatedAt,
            company.ModifiedAt);

        return Result<CompanyDto>.Success(dto);
    }
}
