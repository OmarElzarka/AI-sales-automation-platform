using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Deals.Models;

namespace SalesAI.Application.Features.Deals.Queries;

public record GetDealByIdQuery(Guid Id) : IRequest<Result<DealDto>>;

public class GetDealByIdQueryHandler : IRequestHandler<GetDealByIdQuery, Result<DealDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDealByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DealDto>> Handle(GetDealByIdQuery request, CancellationToken cancellationToken)
    {
        var deal = await _context.Deals
            .Include(d => d.Lead)
            .Include(d => d.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (deal == null)
        {
            return Result<DealDto>.Failure("Deal not found.");
        }

        var dto = new DealDto(
            deal.Id,
            deal.Title,
            deal.Value,
            deal.Currency,
            deal.Stage,
            deal.Stage.ToString(),
            deal.Probability,
            deal.ExpectedCloseDate,
            deal.ActualCloseDate,
            deal.LostReason,
            deal.LeadId,
            deal.Lead != null ? $"{deal.Lead.FirstName} {deal.Lead.LastName}" : null,
            deal.CompanyId,
            deal.Company?.Name,
            deal.OwnerId,
            deal.CreatedAt,
            deal.ModifiedAt);

        return Result<DealDto>.Success(dto);
    }
}
