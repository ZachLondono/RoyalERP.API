using MediatR;
using System.Text.Json.Serialization;

namespace RoyalERP.API.Common.Domain;

/// <summary>
/// A domain event is some action which occurred that affects the domain
/// </summary>
public abstract record DomainEvent : INotification {

    /// <summary>
    /// The id of the aggregate to which this event occurred
    /// </summary>
    [JsonIgnore]
    public Guid AggregateId { get; init; }

    [JsonIgnore]
    public Guid EventId { get; init; }

    /// <summary>
    /// The id of another event which is correlated with this one
    /// </summary>
    [JsonIgnore]
    public Guid? CorrelationId { get; init; }

    [JsonIgnore]
    public bool IsPublished { get; private set; } = false;

    public async Task<bool> Publish(IPublisher publisher) {
        if (IsPublished) return false;
        await publisher.Publish(this);
        IsPublished = true;
        return true;
    }

    /// <param name="aggregateId">The id of the aggregate to which this event occcured</param>
    public DomainEvent(Guid aggregateId) {
        AggregateId = aggregateId;
        EventId = Guid.NewGuid();
    }

    public DomainEvent(Guid aggregateId, Guid? correlationId = null) : this(aggregateId) {
        CorrelationId = correlationId;
    }

}