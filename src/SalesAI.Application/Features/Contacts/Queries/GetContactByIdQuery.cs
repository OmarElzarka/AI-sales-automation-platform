using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Contacts.Models;

namespace SalesAI.Application.Features.Contacts.Queries;

public record GetContactByIdQuery(Guid Id) : IRequest<Result<ContactDto>>;

public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, Result<ContactDto>>
{
    private readonly IApplicationDbContext _context;

    public GetContactByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ContactDto>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var contact = await _context.Contacts
            .Include(c => c.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (contact == null)
        {
            return Result<ContactDto>.Failure("Contact not found.");
        }

        var dto = new ContactDto(
            contact.Id,
            contact.FirstName,
            contact.LastName,
            contact.Email,
            contact.Phone,
            contact.JobTitle,
            contact.IsPrimary,
            contact.CompanyId,
            contact.Company?.Name,
            contact.CreatedAt,
            contact.ModifiedAt);

        return Result<ContactDto>.Success(dto);
    }
}
