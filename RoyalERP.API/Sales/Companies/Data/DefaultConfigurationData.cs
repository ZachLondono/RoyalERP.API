namespace RoyalERP.API.Sales.Companies.Data;

public class DefaultConfigurationData {

    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public Guid ProductId { get; set; }

    public Guid AttributeId { get; set; }

    public string Value { get; set; } = string.Empty;

}