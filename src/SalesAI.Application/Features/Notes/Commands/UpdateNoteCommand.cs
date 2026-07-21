using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Notes.Commands;

public record UpdateNoteCommand(
    Guid Id,
    string Content) : IRequest<Result<Guid>>;

public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Content).NotEmpty();
    }
}

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateNoteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FindAsync(new object[] { request.Id }, cancellationToken);

        if (note == null)
        {
            return Result<Guid>.Failure("Note not found.");
        }

        if (note.AuthorId != _currentUserService.UserId)
        {
            return Result<Guid>.Failure("You are not authorized to edit this note.");
        }

        note.Content = request.Content;
        note.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(note.Id);
    }
}
