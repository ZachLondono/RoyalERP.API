using RoyalERP.API.Common.Domain;
using System.Collections.ObjectModel;
using static RoyalERP.API.Sales.Companies.Domain.Events;

namespace RoyalERP.API.Sales.Companies.Domain;

public class Contact : Entity {

    public Guid CompanyId { get; init; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    private readonly List<string> _roles;
    public ReadOnlyCollection<string> Roles => _roles.AsReadOnly();

    public Contact(Guid id, Guid companyId, string name, string email, string phone, List<string> roles) : base(id) {
        CompanyId = companyId;
        Name = name;
        Email = email;
        Phone = phone;
        _roles = roles.Distinct().ToList();
    }

    private Contact(Guid companyId, string name, string email, string phone, List<string> roles) : this(Guid.NewGuid(), companyId, name, email, phone, roles) {
        AddEvent(new CompanyContactCreated(companyId, Id, name, email, phone));
    }

    public static Contact Create(Guid companyId, string name, string email, string phone, List<string> roles) => new(companyId, name, email, phone, roles);

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

    public void AddRole(string role) {
        if (_roles.Contains(role)) return;
        _roles.Add(role);
        AddEvent(new CompanyContactRoleAdded(CompanyId, Id, role));
    }

    public bool RemoveRole(string role) {
        var result = _roles.Remove(role);
        if (result) {
            AddEvent(new CompanyContactRoleRemoved(CompanyId, Id, role));
        }
        return result;
    }

}
