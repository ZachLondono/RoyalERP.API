using MediatR;
using System.Text.Json.Serialization;

namespace RoyalERP.Common.Domain;

/// <summary>
/// An entity is a uniquely identifiable domain concept
/// </summary>
public abstract class Entity {

    public Guid Id { get; init; }

    protected List<DomainEvent> _events = new();

    public Entity(Guid id) => Id = id;

    [JsonIgnore]
    public IEnumerable<DomainEvent> Events => _events;

    public void ClearEvents() => _events.Clear();

    protected void AddEvent(DomainEvent domainEvent) => _events.Add(domainEvent);

    public virtual async Task<int> PublishEvents(IPublisher publisher) {
        int publishedCount = 0;
        foreach (DomainEvent domainEvent in _events) {
            if (await domainEvent.Publish(publisher)) {
                publishedCount++;
            }
        }
        return publishedCount;
    }

}