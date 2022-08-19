using RoyalERP.Common.Domain;

namespace RoyalERP.API.Catalog.ProductClasses.Domain;

public static class Events {

    public record ProductClassEvent(Guid ClassId) : DomainEvent(ClassId);

    public record ProductClassCreated(Guid ClassId, string Name) : ProductClassEvent(ClassId);

    public record ProductClassUpdated(Guid ClassId, string Name) : ProductClassEvent(ClassId);

    public record ProductClassDeleted(Guid ClassId) : ProductClassEvent(ClassId);

}
