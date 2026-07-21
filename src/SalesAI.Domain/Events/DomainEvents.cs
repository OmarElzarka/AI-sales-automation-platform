using SalesAI.Domain.Common;
using SalesAI.Domain.Enums;

namespace SalesAI.Domain.Events;

public record LeadCreatedEvent(Guid LeadId) : DomainEvent;

public record LeadScoredEvent(Guid LeadId, LeadScoreCategory Category, int NumericScore) : DomainEvent;

public record DealStageChangedEvent(
    Guid DealId,
    DealStage FromStage,
    DealStage ToStage,
    string ChangedBy) : DomainEvent;

public record TaskCompletedEvent(Guid TaskId, Guid CompletedBy) : DomainEvent;
