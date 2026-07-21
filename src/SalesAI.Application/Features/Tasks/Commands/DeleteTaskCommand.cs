using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Tasks.Commands;

public record DeleteTaskCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.SalesTasks.FindAsync(new object[] { request.Id }, cancellationToken);

        if (task == null)
        {
            return Result<bool>.Failure("Task not found.");
        }

        _context.SalesTasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
