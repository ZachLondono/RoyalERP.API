namespace RoyalERP.API.Contracts.Products;

public class ProductDTO {

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid? ClassId { get; set; }

    public IEnumerable<Guid> Attributes { get; set; } = Enumerable.Empty<Guid>();

}
