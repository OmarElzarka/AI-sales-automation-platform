using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Entities;

namespace SalesAI.Application.Features.Notes.Commands;

public record CreateNoteCommand(
    string Content,
    Guid? LeadId,
    Guid? DealId,
    Guid AuthorId) : IRequest<Result<Guid>>;

public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(v => v.Content).NotEmpty();
        RuleFor(v => v.AuthorId).NotEmpty();
        RuleFor(v => v)
            .Must(v => v.LeadId.HasValue || v.DealId.HasValue)
            .WithMessage("A note must be associated with a Lead or a Deal.");
    }
}

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new Note
        {
            Content = request.Content,
            LeadId = request.LeadId,
            DealId = request.DealId,
            AuthorId = request.AuthorId
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(note.Id);
    }
}
