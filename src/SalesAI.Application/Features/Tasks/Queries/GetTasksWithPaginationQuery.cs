using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Tasks.Models;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Tasks.Queries;

public record GetTasksWithPaginationQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? AssignedToId = null,
    Guid? LeadId = null,
    Guid? DealId = null,
    SalesTaskStatus? Status = null,
    bool? IsOverdue = null,
    string? SearchTerm = null) : IRequest<Result<PaginatedList<TaskDto>>>;

public class GetTasksWithPaginationQueryHandler : IRequestHandler<GetTasksWithPaginationQuery, Result<PaginatedList<TaskDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetTasksWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<TaskDto>>> Handle(GetTasksWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SalesTasks.AsNoTracking().AsQueryable();

        if (request.AssignedToId.HasValue)
            query = query.Where(t => t.AssignedToId == request.AssignedToId.Value);

        if (request.LeadId.HasValue)
            query = query.Where(t => t.LeadId == request.LeadId.Value);

        if (request.DealId.HasValue)
            query = query.Where(t => t.DealId == request.DealId.Value);

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.IsOverdue.HasValue && request.IsOverdue.Value)
        {
            var now = DateTime.UtcNow;
            query = query.Where(t => t.Status != SalesTaskStatus.Completed && 
                                     t.Status != SalesTaskStatus.Cancelled && 
                                     t.DueDate < now);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(t => 
                t.Title.ToLower().Contains(searchTerm) || 
                (t.Description != null && t.Description.ToLower().Contains(searchTerm)));
        }

        query = query.OrderBy(t => t.DueDate);

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TaskDto(
                t.Id,
                t.Title,
                t.Description,
                t.Type,
                t.Type.ToString(),
                t.Priority,
                t.Priority.ToString(),
                t.Status,
                t.Status.ToString(),
                t.DueDate,
                t.CompletedAt,
                t.ReminderSent,
                t.IsOverdue,
                t.AssignedToId,
                t.LeadId,
                t.DealId,
                t.ContactId,
                t.CreatedAt,
                t.ModifiedAt))
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<TaskDto>(items, count, request.PageNumber, request.PageSize);

        return Result<PaginatedList<TaskDto>>.Success(paginatedList);
    }
}
