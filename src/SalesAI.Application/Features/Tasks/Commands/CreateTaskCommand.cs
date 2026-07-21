using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Tasks.Commands;

public record CreateTaskCommand(
    string Title,
    string? Description,
    TaskType Type,
    TaskPriority Priority,
    DateTime DueDate,
    Guid AssignedToId,
    Guid? LeadId,
    Guid? DealId,
    Guid? ContactId) : IRequest<Result<Guid>>;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(v => v.Title).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Description).MaximumLength(2000);
        RuleFor(v => v.Type).IsInEnum();
        RuleFor(v => v.Priority).IsInEnum();
        RuleFor(v => v.DueDate).NotEmpty();
        RuleFor(v => v.AssignedToId).NotEmpty();
    }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new SalesTask
        {
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Priority = request.Priority,
            DueDate = request.DueDate,
            AssignedToId = request.AssignedToId,
            LeadId = request.LeadId,
            DealId = request.DealId,
            ContactId = request.ContactId,
            Status = SalesTaskStatus.Pending,
            ReminderSent = false
        };

        _context.SalesTasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(task.Id);
    }
}
