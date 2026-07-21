using FluentValidation;
using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Application.Common.Models;

namespace SalesAI.Application.Features.Deals.Commands;

public record UpdateDealCommand(
    Guid Id,
    string Title,
    decimal Value,
    string Currency,
    int Probability,
    DateTime? ExpectedCloseDate,
    Guid? LeadId,
    Guid? CompanyId,
    Guid OwnerId) : IRequest<Result<Guid>>;

public class UpdateDealCommandValidator : AbstractValidator<UpdateDealCommand>
{
    public UpdateDealCommandValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.Title).NotEmpty().MaximumLength(200);
        RuleFor(v => v.Value).GreaterThanOrEqualTo(0);
        RuleFor(v => v.Currency).NotEmpty().Length(3);
        RuleFor(v => v.Probability).InclusiveBetween(0, 100);
        RuleFor(v => v.OwnerId).NotEmpty();
    }
}

public class UpdateDealCommandHandler : IRequestHandler<UpdateDealCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public UpdateDealCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(UpdateDealCommand request, CancellationToken cancellationToken)
    {
        var deal = await _context.Deals.FindAsync(new object[] { request.Id }, cancellationToken);

        if (deal == null)
        {
            return Result<Guid>.Failure("Deal not found.");
        }

        deal.Title = request.Title;
        deal.Value = request.Value;
        deal.Currency = request.Currency;
        deal.Probability = request.Probability;
        deal.ExpectedCloseDate = request.ExpectedCloseDate;
        deal.LeadId = request.LeadId;
        deal.CompanyId = request.CompanyId;
        deal.OwnerId = request.OwnerId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(deal.Id);
    }
}
