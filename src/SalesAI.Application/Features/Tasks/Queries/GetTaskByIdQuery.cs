using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Tasks.Models;

namespace SalesAI.Application.Features.Tasks.Queries;

public record GetTaskByIdQuery(Guid Id) : IRequest<Result<TaskDto>>;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTaskByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.SalesTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task == null)
        {
            return Result<TaskDto>.Failure("Task not found.");
        }

        var dto = new TaskDto(
            task.Id,
            task.Title,
            task.Description,
            task.Type,
            task.Type.ToString(),
            task.Priority,
            task.Priority.ToString(),
            task.Status,
            task.Status.ToString(),
            task.DueDate,
            task.CompletedAt,
            task.ReminderSent,
            task.IsOverdue,
            task.AssignedToId,
            task.LeadId,
            task.DealId,
            task.ContactId,
            task.CreatedAt,
            task.ModifiedAt);

        return Result<TaskDto>.Success(dto);
    }
}
