using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Deals.Commands;

public record DeleteDealCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteDealCommandHandler : IRequestHandler<DeleteDealCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteDealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteDealCommand request, CancellationToken cancellationToken)
    {
        var deal = await _context.Deals.FindAsync(new object[] { request.Id }, cancellationToken);

        if (deal == null)
        {
            return Result<bool>.Failure("Deal not found.");
        }

        _context.Deals.Remove(deal);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
