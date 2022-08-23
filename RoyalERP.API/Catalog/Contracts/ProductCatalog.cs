namespace RoyalERP.API.Catalog.Contracts;

public class ProductCatalog {

    public delegate Task<Guid?> GetProductClassByProductId(Guid productId);

}
