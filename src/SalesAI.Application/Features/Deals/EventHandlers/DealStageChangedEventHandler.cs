using MediatR;
using SalesAI.Application.Common.Interfaces;
using SalesAI.Domain.Entities;
using SalesAI.Domain.Events;

namespace SalesAI.Application.Features.Deals.EventHandlers;

public class DealStageChangedEventHandler : INotificationHandler<DealStageChangedEvent>
{
    private readonly IApplicationDbContext _context;

    public DealStageChangedEventHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DealStageChangedEvent notification, CancellationToken cancellationToken)
    {
        var history = new DealStageHistory
        {
            DealId = notification.DealId,
            FromStage = notification.FromStage,
            ToStage = notification.ToStage,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = notification.ChangedBy
        };

        _context.DealStageHistory.Add(history);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
