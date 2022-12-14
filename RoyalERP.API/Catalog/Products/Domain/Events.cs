using RoyalERP.API.Common.Domain;

namespace RoyalERP.API.Catalog.Products.Domain;

public static class Events {

    public record ProductEvent(Guid ProductId) : DomainEvent(ProductId);

    public record ProductCreated(Guid ProductId, string Name) : ProductEvent(ProductId);

    public record ProductNameUpdated(Guid ProductId, string Name) : ProductEvent(ProductId);

    public record ProductAttributeAdded(Guid ProductId, Guid AttributeId) : ProductEvent(ProductId);

    public record ProductAttributeRemoved(Guid ProductId, Guid AttributeId) : ProductEvent(ProductId);

    public record ProductDeleted(Guid ProductId) : ProductEvent(ProductId);

    public record ProductAddedToClass(Guid ProductId, Guid ProductClassId) : ProductEvent(ProductId);

    public record ProductRemovedFromClass(Guid ProductId) : ProductEvent(ProductId);

}
