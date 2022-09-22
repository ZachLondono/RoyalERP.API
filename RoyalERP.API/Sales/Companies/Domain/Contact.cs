using RoyalERP.API.Common.Domain;
using static RoyalERP.API.Sales.Companies.Domain.Events;

namespace RoyalERP.API.Sales.Companies.Domain;

public class Contact : Entity {

    public Guid CompanyId { get; init; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public Contact(Guid id, Guid companyId, string name, string email, string phone) : base(id) {
        CompanyId = companyId;
        Name = name;
        Email = email;
        Phone = phone;
    }

    private Contact(Guid companyId, string name, string email, string phone) : this(Guid.NewGuid(), companyId, name, email, phone) {
        AddEvent(new CompanyContactCreated(companyId, Id, name, email, phone));
    }

    public static Contact Create(Guid companyId, string name, string email, string phone) => new(companyId, name, email, phone);

    public void SetName(string name) {
        Name = name;
        AddEvent(new CompanyContactNameUpdated(CompanyId, Id, name));
    }

    public void SetEmail(string email) {
        Email = email;
        AddEvent(new CompanyContactEmailUpdated(CompanyId, Id, email));
    }

    public void SetPhone(string phone) {
        Phone = phone;
        AddEvent(new CompanyContactPhoneUpdated(CompanyId, Id, phone));
    }

}
