using RoyalERP.Common.Domain;

namespace RoyalERP.Sales.Companies.Domain;

public class Company : AggregateRoot {

    public string Name { get; private set; }

    public string Contact { get; private set; }

    public string Email { get; private set; }

    public Address Address { get; private set; }

    public Company(Guid id, int version, string name, string contact, string email, Address address) : base(id, version) {
        Name = name;
        Contact = contact;
        Email = email;
        Address = address;
    }

    private Company(string name) : this(Guid.NewGuid(), 0, name, "", "", new()) {
        AddEvent(new Events.CompanyCreatedEvent(Id, name));
    }

    public static Company Create(string name) => new(name);

    public void Update(string name, string contact, string email) {
        Name = name;
        Contact = contact;
        Email = email;
        AddEvent(new Events.CompanyUpdatedEvent(Id, name, contact, email));
    }

    public void SetAddress(string line1, string line2, string line3, string city, string state, string zip) {
        Address.Line1 = line1;
        Address.Line2 = line2;
        Address.Line3 = line3;
        Address.City = city;
        Address.State = state;
        Address.Zip = zip;
        AddEvent(new Events.CompanyAddressUpdatedEvent(Id, line1, line2, line3, city, state, zip));
    }

}