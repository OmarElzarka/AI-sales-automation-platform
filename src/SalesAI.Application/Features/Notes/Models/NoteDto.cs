namespace SalesAI.Application.Features.Notes.Models;

public record NoteDto(
    Guid Id,
    string Content,
    DateTime CreatedAt,
    DateTime? ModifiedAt,
    Guid? LeadId,
    Guid? DealId,
    Guid AuthorId,
    string AuthorName);
