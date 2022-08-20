using RoyalERP.API.Catalog.ProductAttributes.Domain;
using RoyalERP.API.Catalog.ProductClasses.Domain;
using RoyalERP.API.Catalog.Products.Domain;
using RoyalERP.Common.Domain;

namespace RoyalERP.API.Catalog;

public interface ICatalogUnitOfWork : IUnitOfWork {

    IProductRepository Products { get; }
    IProductClassRepository ProductClasses { get; }
    IProductAttributeRepository ProductAttributes { get; }

}
