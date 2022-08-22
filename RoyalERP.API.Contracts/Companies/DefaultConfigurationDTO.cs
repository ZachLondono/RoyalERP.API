namespace RoyalERP.API.Contracts.Companies;

public class DefaultConfigurationDTO {

    public Guid ProductId { get; set; }

    public Guid AttributeId { get; set; }

    public string Value { get; set; } = string.Empty;

}