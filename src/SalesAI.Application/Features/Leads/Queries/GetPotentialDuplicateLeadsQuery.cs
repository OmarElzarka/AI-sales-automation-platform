using MediatR;
using SalesAI.Application.Common.Models;
using SalesAI.Application.Features.Leads.Models;
using SalesAI.Application.Features.Leads.Services;

namespace SalesAI.Application.Features.Leads.Queries;

public record GetPotentialDuplicateLeadsQuery(
    string Email,
    string? Phone,
    string FirstName,
    string LastName) : IRequest<Result<List<LeadDto>>>;

public class GetPotentialDuplicateLeadsQueryHandler : IRequestHandler<GetPotentialDuplicateLeadsQuery, Result<List<LeadDto>>>
{
    private readonly ILeadDuplicateDetectionService _duplicateDetectionService;

    public GetPotentialDuplicateLeadsQueryHandler(ILeadDuplicateDetectionService duplicateDetectionService)
    {
        _duplicateDetectionService = duplicateDetectionService;
    }

    public async Task<Result<List<LeadDto>>> Handle(GetPotentialDuplicateLeadsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Phone) && 
            (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName)))
        {
            return Result<List<LeadDto>>.Failure("Insufficient data to find duplicates. Provide Email, Phone, or First and Last Name.");
        }

        var duplicates = await _duplicateDetectionService.FindPotentialDuplicatesAsync(
            request.Email, 
            request.Phone, 
            request.FirstName, 
            request.LastName, 
            cancellationToken);

        return Result<List<LeadDto>>.Success(duplicates);
    }
}
