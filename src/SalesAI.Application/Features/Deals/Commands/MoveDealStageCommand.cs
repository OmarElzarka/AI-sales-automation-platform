using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Deals.Commands;

public record MoveDealStageCommand(Guid DealId, DealStage NewStage, string? LostReason) : IRequest<Result<bool>>;

public class MoveDealStageCommandHandler : IRequestHandler<MoveDealStageCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public MoveDealStageCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(MoveDealStageCommand request, CancellationToken cancellationToken)
    {
        var deal = await _context.Deals.FindAsync(new object[] { request.DealId }, cancellationToken);

        if (deal == null)
        {
            return Result<bool>.Failure("Deal not found.");
        }

        var userId = _currentUserService.UserId != Guid.Empty ? _currentUserService.UserId.ToString() : "System";

        deal.MoveToStage(request.NewStage, userId);

        if (request.NewStage == DealStage.Lost)
        {
            deal.LostReason = request.LostReason;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
