using RoyalERP.API.Common.Domain;
using System.Text.Json.Serialization;

namespace RoyalERP.API.Sales.Companies.Domain;

public static class Events {

    public abstract record CompanyEvent([property: JsonIgnore] Guid CompanyId) : DomainEvent(CompanyId);

    public record CompanyCreatedEvent(Guid CompanyId, string Name) : CompanyEvent(CompanyId);

    public record CompanyUpdatedEvent(Guid CompanyId, string Name, string Contact, string Email) : CompanyEvent(CompanyId);

    public record CompanyAddressUpdatedEvent(Guid CompanyId, string Line1, string Line2, string Line3, string City, string State, string Zip) : CompanyEvent(CompanyId);

    public record CompanyDefaultAddedEvent(Guid CompanyId, Guid DefaultId, Guid ProductId, Guid AttributeId, string Value) : CompanyEvent(CompanyId);

    public record CompanyDefaultRemovedEvent(Guid CompanyId, Guid DefaultId) : CompanyEvent(CompanyId);

    public record CompanyDefaultUpdatedEvent(Guid CompanyId, Guid DefaultId, string Value) : CompanyEvent(CompanyId);

    public record CompanyInfoFieldSetEvent(Guid CompanyId, string Field, string Value) : CompanyEvent(CompanyId);

    public record CompanyInfoFieldRemovedEvent(Guid CompanyId, string Field) : CompanyEvent(CompanyId);

}
