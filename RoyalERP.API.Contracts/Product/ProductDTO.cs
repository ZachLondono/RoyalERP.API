namespace RoyalERP.API.Contracts.Product;

public class ProductDTO {

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IEnumerable<Guid> Attributes { get; set; } = Enumerable.Empty<Guid>();

}
