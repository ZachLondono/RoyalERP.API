using MediatR;

namespace RoyalERP.API.Common.Domain;

/// <summary>
/// An aggregate root is an entity which is the root of it's aggregate. All other entities in the aggregate are referenced through the root.
/// </summary>
public abstract class AggregateRoot : Entity {
    
    public int Version { get; private set; }

    protected AggregateRoot(Guid id, int version)
        : base(id)
        => Version = version;

    public override async Task<int> PublishEvents(IPublisher publisher) {
        int published = await base.PublishEvents(publisher);
        Version += published;
        return published;
    } 

}
