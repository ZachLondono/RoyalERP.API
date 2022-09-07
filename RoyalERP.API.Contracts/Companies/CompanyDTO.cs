namespace RoyalERP.API.Contracts.Companies;

public class CompanyDTO {

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Contact { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public AddressDTO Address { get; set; } = new AddressDTO();

    public IEnumerable<DefaultConfigurationDTO> Defaults { get; set; } = Enumerable.Empty<DefaultConfigurationDTO>();

    public Dictionary<string, string> Info { get; set; } = new();
 
}