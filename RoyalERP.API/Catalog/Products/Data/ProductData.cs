namespace RoyalERP.API.Catalog.Products.Data;

public class ProductData {

    public Guid Id { get; set; }

    public int Version { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid[] AttributeIds { get; set; } = Array.Empty<Guid>();

}
