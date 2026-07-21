using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Leads.Models;

namespace SalesAI.Application.Features.Leads.Queries;

public record GetLeadByIdQuery(Guid Id) : IRequest<Result<LeadDto>>;

public class GetLeadByIdQueryHandler : IRequestHandler<GetLeadByIdQuery, Result<LeadDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLeadByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LeadDto>> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
    {
        var lead = await _context.Leads
            .Include(l => l.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (lead == null)
        {
            return Result<LeadDto>.Failure("Lead not found.");
        }

        var dto = new LeadDto(
            lead.Id,
            lead.FirstName,
            lead.LastName,
            lead.Email,
            lead.Phone,
            lead.Company?.Name,
            lead.JobTitle,
            lead.Status.ToString(),
            lead.Source.ToString(),
            lead.AssignedToId,
            lead.CompanyId,
            lead.ScoreNumeric,
            lead.CreatedAt,
            lead.ModifiedAt);

        return Result<LeadDto>.Success(dto);
    }
}
