using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Tasks.Commands;

public record UpdateTaskCommand(
    Guid Id,
    string Title,
    string? Description,
    TaskType Type,
    TaskPriority Priority,
    SalesTaskStatus Status,
    DateTime DueDate,
    Guid AssignedToId,
    Guid? LeadId,
    Guid? DealId,
    Guid? ContactId) : IRequest<Result<Guid>>;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Title).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Description).MaximumLength(2000);
        RuleFor(v => v.Type).IsInEnum();
        RuleFor(v => v.Priority).IsInEnum();
        RuleFor(v => v.Status).IsInEnum();
        RuleFor(v => v.DueDate).NotEmpty();
        RuleFor(v => v.AssignedToId).NotEmpty();
    }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.SalesTasks.FindAsync(new object[] { request.Id }, cancellationToken);

        if (task == null)
        {
            return Result<Guid>.Failure("Task not found.");
        }

        task.Title = request.Title;
        task.Description = request.Description;
        task.Type = request.Type;
        task.Priority = request.Priority;
        task.Status = request.Status;
        task.DueDate = request.DueDate;
        task.AssignedToId = request.AssignedToId;
        task.LeadId = request.LeadId;
        task.DealId = request.DealId;
        task.ContactId = request.ContactId;

        if (request.Status == SalesTaskStatus.Completed && task.CompletedAt == null)
        {
            task.Complete();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(task.Id);
    }
}
