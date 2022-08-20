using RoyalERP.API.Common.Domain;

namespace RoyalERP.API.Catalog.Products.Domain;

public class Product : AggregateRoot {

    public string Name { get; private set; }

    public Guid? ClassId { get; private set; }

    private readonly List<Guid> _attributeIds;
    public IReadOnlyCollection<Guid> AttributeIds => _attributeIds.AsReadOnly();

    public Product(Guid id, int version, string name, Guid? classId, List<Guid> attributes) : base(id, version) {
        Name = name;
        ClassId = classId;
        _attributeIds = attributes;
    }

    private Product(string name) : this(Guid.NewGuid(), 0, name, null, new()) {
        AddEvent(new Events.ProductCreated(Id, name));
    }

    public static Product Create(string name) => new(name);

    public void SetName(string name) {
        Name = name;
        AddEvent(new Events.ProductNameUpdated(Id, name));
    }

    public void AddAttribute(Guid attributeId) {
        if (_attributeIds.Contains(attributeId)) return;
        _attributeIds.Add(attributeId);
        AddEvent(new Events.ProductAttributeAdded(Id, attributeId));
    }

    public bool RemoveAttribute(Guid attributeId) {
        var result = _attributeIds.Remove(attributeId);
        if (result) AddEvent(new Events.ProductAttributeRemoved(Id, attributeId));
        return result;
    }

    public void RemoveFromProductClass() {
        if (ClassId is null) return;
        ClassId = null;
        AddEvent(new Events.ProductRemovedFromClass(Id));
    }

    public void SetProductClass(Guid classId) {
        ClassId = classId;
        AddEvent(new Events.ProductAddedToClass(Id, classId));
    }

}
