using RoyalERP.Common.Domain;

namespace RoyalERP.Sales.Companies.Domain;

public class Company : AggregateRoot {

    public string Name { get; private set; }

    public Company(Guid id, int version, string name) : base(id, version) {
        Name = name;
    }

    private Company(string name) : this(Guid.NewGuid(), 0, name) {
        AddEvent(new Events.CompanyCreatedEvent(Id, name));
    }

    public static Company Create(string name) => new(name);

}
