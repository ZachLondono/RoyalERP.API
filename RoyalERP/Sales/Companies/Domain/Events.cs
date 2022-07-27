using RoyalERP.Common.Domain;
using System.Text.Json.Serialization;

namespace RoyalERP.Sales.Companies.Domain;

public static class Events {

    public abstract record CompanyEvent([property: JsonIgnore] Guid CompanyId) : DomainEvent(CompanyId);

    public record CompanyCreatedEvent(Guid CompanyId, string Name) : CompanyEvent(CompanyId);

}
