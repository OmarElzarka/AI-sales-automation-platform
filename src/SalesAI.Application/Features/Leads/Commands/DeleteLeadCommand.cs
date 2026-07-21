using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Leads.Commands;

public record DeleteLeadCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteLeadCommandHandler : IRequestHandler<DeleteLeadCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteLeadCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteLeadCommand request, CancellationToken cancellationToken)
    {
        var lead = await _context.Leads.FindAsync(new object[] { request.Id }, cancellationToken);

        if (lead == null)
        {
            return Result<bool>.Failure("Lead not found.");
        }

        _context.Leads.Remove(lead);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
