using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Enums;

namespace SalesAI.Application.Features.Deals.Commands;

public record CreateDealCommand(
    string Title,
    decimal Value,
    string Currency,
    DealStage Stage,
    int Probability,
    DateTime? ExpectedCloseDate,
    Guid? LeadId,
    Guid? CompanyId,
    Guid OwnerId) : IRequest<Result<Guid>>;

public class CreateDealCommandValidator : AbstractValidator<CreateDealCommand>
{
    public CreateDealCommandValidator()
    {
        RuleFor(v => v.Title).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Value).GreaterThanOrEqualTo(0);
        RuleFor(v => v.Currency).NotEmpty().Length(3);
        RuleFor(v => v.Stage).IsInEnum();
        RuleFor(v => v.Probability).InclusiveBetween(0, 100);
        RuleFor(v => v.OwnerId).NotEmpty();
    }
}

public class CreateDealCommandHandler : IRequestHandler<CreateDealCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateDealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateDealCommand request, CancellationToken cancellationToken)
    {
        var deal = new Deal
        {
            Title = request.Title,
            Value = request.Value,
            Currency = request.Currency,
            Probability = request.Probability,
            ExpectedCloseDate = request.ExpectedCloseDate,
            LeadId = request.LeadId,
            CompanyId = request.CompanyId,
            OwnerId = request.OwnerId
        };

        // Initialize Stage using MoveToStage to record initial stage history if needed.
        // Wait, Deal entity defaults to NewLead. If it's created in a different stage or just to track creation:
        deal.MoveToStage(request.Stage, request.OwnerId.ToString());

        _context.Deals.Add(deal);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(deal.Id);
    }
}
