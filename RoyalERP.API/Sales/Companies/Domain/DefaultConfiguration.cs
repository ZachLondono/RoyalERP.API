using RoyalERP.API.Common.Domain;

namespace RoyalERP.API.Sales.Companies.Domain;

public class DefaultConfiguration : Entity {

    public Guid CompanyId { get; init; }

    public Guid ProductId { get; init; }

    public Guid AttributeId { get; init; }

    public string Value { get; private set; } = string.Empty;

    public DefaultConfiguration(Guid id, Guid companyId, Guid productId, Guid attributeId, string value) : base(id) {
        CompanyId = companyId;
        ProductId = productId;
        AttributeId = attributeId;
        Value = value;
    }

    private DefaultConfiguration(Guid companyId, Guid productId, Guid attributeId, string value) : this(Guid.NewGuid(), companyId, productId, attributeId, value) {
        AddEvent(new Events.CompanyDefaultAddedEvent(companyId, Id, productId, attributeId, value));
    }

    public static DefaultConfiguration Create(Guid companyId, Guid productId, Guid attributeId, string value) => new(companyId, productId, attributeId, value);

    public void SetValue(string value) {
        Value = value;
        AddEvent(new Events.CompanyDefaultUpdatedEvent(CompanyId, Id, value));
    }

}