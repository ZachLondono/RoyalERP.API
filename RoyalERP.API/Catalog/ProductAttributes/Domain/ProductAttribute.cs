using RoyalERP.API.Common.Domain;

namespace RoyalERP.API.Catalog.ProductAttributes.Domain;

public class ProductAttribute : AggregateRoot {

    public string Name { get; private set; }

    public ProductAttribute(Guid id, int version, string name) : base(id, version) {
        Name = name;
    }

    private ProductAttribute(string name) : this(Guid.NewGuid(), 0, name) {
        AddEvent(new Events.ProductAttributeCreated(Id, name));
    }

    public static ProductAttribute Create(string name) => new(name);

    public void SetName(string name) {
        Name = name;
        AddEvent(new Events.ProductAttributeUpdated(Id, name));
    }

}