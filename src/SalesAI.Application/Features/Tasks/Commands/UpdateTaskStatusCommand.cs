using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Tasks.Commands;

public record UpdateTaskStatusCommand(Guid Id, string Status) : IRequest<Result<bool>>;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTaskStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.SalesTasks.FindAsync(new object[] { request.Id }, cancellationToken);

        if (task == null)
            return Result<bool>.Failure("Task not found.");

        if (Enum.TryParse<SalesTaskStatus>(request.Status, true, out var status))
        {
            task.Status = status;

            if (status == SalesTaskStatus.Completed)
            {
                task.Complete();
            }
            else
            {
                task.CompletedAt = null;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

        return Result<bool>.Failure("Invalid status.");
    }
}
