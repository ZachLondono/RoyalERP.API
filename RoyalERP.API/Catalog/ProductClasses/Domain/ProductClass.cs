using RoyalERP.Common.Domain;

namespace RoyalERP.API.Catalog.ProductClasses.Domain;

public class ProductClass : AggregateRoot {

    public string Name { get; private set; }

    public ProductClass(Guid id, int version, string name) : base(id, version) {
        Name = name;
    }

    private ProductClass(string name) : this(Guid.NewGuid(), 0, name) {
        AddEvent(new Events.ProductClassCreated(Id, name));
    }

    public static ProductClass Create(string name) => new(name); 

    public void SetName(string name) {
        Name = name;
        AddEvent(new Events.ProductClassUpdated(Id, name));
    }

}