using RoyalERP.Common.Domain;

namespace RoyalERP.API.Catalog.ProductAttributes.Domain;

public static class Events {

    public record ProductAttributeEvent(Guid AttributeId) : DomainEvent(AttributeId);

    public record ProductAttributeCreated(Guid AttributeId, string Name) : ProductAttributeEvent(AttributeId);

    public record ProductAttributeUpdated(Guid AttributeId, string Name) : ProductAttributeEvent(AttributeId);

    public record ProductAttributeDeleted(Guid AttributeId) : ProductAttributeEvent(AttributeId);

}
