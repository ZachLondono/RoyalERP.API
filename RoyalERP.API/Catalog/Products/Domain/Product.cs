using RoyalERP.Common.Domain;
using System.Xml.Linq;

namespace RoyalERP.API.Catalog.Products.Domain;

public class Product : AggregateRoot {

    public string Name { get; private set; }

    private readonly List<Guid> _attributeIds;
    public IReadOnlyCollection<Guid> AttributeIds => _attributeIds.AsReadOnly();

    public Product(Guid id, int version, string name, List<Guid> attributes) : base(id, version) {
        Name = name;
        _attributeIds = attributes;
    }

    private Product(string name) : this(Guid.NewGuid(), 0, name, new()) {
        AddEvent(new Events.ProductCreated(Id, name));
    }

    public static Product Create(string name) => new(name);

    public void SetName(string name) {
        Name = name;
        AddEvent(new Events.ProductNameUpdated(Id, name));
    }

    public void AddAttribute(Guid attributeId) {
        _attributeIds.Add(attributeId);
        AddEvent(new Events.ProductAttributeAdded(Id, attributeId));
    }

    public void RemoveAttribute(Guid attributeId) {
        _attributeIds.Remove(attributeId);
        AddEvent(new Events.ProductAttributeRemoved(Id, attributeId));
    }

}
