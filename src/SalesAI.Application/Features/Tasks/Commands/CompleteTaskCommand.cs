using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Tasks.Commands;

public record CompleteTaskCommand(Guid Id) : IRequest<Result<bool>>;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public CompleteTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.SalesTasks.FindAsync(new object[] { request.Id }, cancellationToken);

        if (task == null)
        {
            return Result<bool>.Failure("Task not found.");
        }

        task.Complete();

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
