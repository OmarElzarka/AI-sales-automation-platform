using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Companies.Commands;

public record DeleteCompanyCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(new object[] { request.Id }, cancellationToken);

        if (company == null)
        {
            return Result<bool>.Failure("Company not found.");
        }

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
