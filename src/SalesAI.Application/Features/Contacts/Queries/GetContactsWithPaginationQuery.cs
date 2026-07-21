using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Contacts.Models;

namespace SalesAI.Application.Features.Contacts.Queries;

public record GetContactsWithPaginationQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CompanyId = null,
    string? SearchTerm = null) : IRequest<Result<PaginatedList<ContactDto>>>;

public class GetContactsWithPaginationQueryHandler : IRequestHandler<GetContactsWithPaginationQuery, Result<PaginatedList<ContactDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetContactsWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ContactDto>>> Handle(GetContactsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Contacts.Include(c => c.Company).AsNoTracking().AsQueryable();

        if (request.CompanyId.HasValue)
        {
            query = query.Where(c => c.CompanyId == request.CompanyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(c => 
                c.FirstName.ToLower().Contains(searchTerm) || 
                c.LastName.ToLower().Contains(searchTerm) || 
                (c.Email != null && c.Email.ToLower().Contains(searchTerm)));
        }

        query = query.OrderBy(c => c.FirstName).ThenBy(c => c.LastName);

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ContactDto(
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.Phone,
                c.JobTitle,
                c.IsPrimary,
                c.CompanyId,
                c.Company != null ? c.Company.Name : null,
                c.CreatedAt,
                c.ModifiedAt))
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<ContactDto>(items, count, request.PageNumber, request.PageSize);

        return Result<PaginatedList<ContactDto>>.Success(paginatedList);
    }
}
