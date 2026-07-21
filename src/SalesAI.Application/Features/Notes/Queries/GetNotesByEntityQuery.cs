using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Notes.Models;

namespace SalesAI.Application.Features.Notes.Queries;

public record GetNotesByEntityQuery(Guid? LeadId = null, Guid? DealId = null) : IRequest<Result<List<NoteDto>>>;

public class GetNotesByEntityQueryHandler : IRequestHandler<GetNotesByEntityQuery, Result<List<NoteDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetNotesByEntityQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<NoteDto>>> Handle(GetNotesByEntityQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Notes.Include(n => n.Author).AsNoTracking().AsQueryable();

        if (request.LeadId.HasValue)
        {
            query = query.Where(n => n.LeadId == request.LeadId.Value);
        }
        else if (request.DealId.HasValue)
        {
            query = query.Where(n => n.DealId == request.DealId.Value);
        }
        else
        {
            return Result<List<NoteDto>>.Failure("Must provide either LeadId or DealId.");
        }

        var notes = await query
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NoteDto(
                n.Id,
                n.Content,
                n.CreatedAt,
                n.ModifiedAt,
                n.LeadId,
                n.DealId,
                n.AuthorId,
                $"{n.Author.FirstName} {n.Author.LastName}"))
            .ToListAsync(cancellationToken);

        return Result<List<NoteDto>>.Success(notes);
    }
}
