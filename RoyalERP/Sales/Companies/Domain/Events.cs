using RoyalERP.Common.Domain;
using System.Text.Json.Serialization;

namespace RoyalERP.Sales.Companies.Domain;

public static class Events {

    public abstract record CompanyEvent([property: JsonIgnore] Guid CompanyId) : DomainEvent(CompanyId);

    public record CompanyCreatedEvent(Guid CompanyId, string Name) : CompanyEvent(CompanyId);

    public record CompanyUpdatedEvent(Guid CompanyId, string Name, string Contact, string Email) : CompanyEvent(CompanyId);

    public record CompanyAddressUpdatedEvent(Guid CompanyId, string Line1, string Line2, string Line3, string City, string State, string Zip) : CompanyEvent(CompanyId);

}
