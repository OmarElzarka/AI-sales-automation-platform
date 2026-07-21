using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Notes.Commands;

public record DeleteNoteCommand(Guid Id) : IRequest<Result<bool>>;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteNoteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.Notes.FindAsync(new object[] { request.Id }, cancellationToken);

        if (note == null)
        {
            return Result<bool>.Failure("Note not found.");
        }

        if (note.AuthorId != _currentUserService.UserId)
        {
            return Result<bool>.Failure("You are not authorized to delete this note.");
        }

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
