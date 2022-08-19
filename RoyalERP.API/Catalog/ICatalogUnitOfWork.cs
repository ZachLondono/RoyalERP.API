using RoyalERP.API.Catalog.Products.Domain;

namespace RoyalERP.API.Catalog;

public interface ICatalogUnitOfWork {

    IProductRepository Products { get; }

}
