using MediatR;
using RoyalERP.API.Catalog.ProductAttributes.Domain;
using RoyalERP.API.Catalog.ProductClasses.Domain;
using RoyalERP.API.Catalog.Products.Domain;
using RoyalERP.API.Common.Data;
using RoyalERP.API.Common.Domain;
using System.Data;

namespace RoyalERP.API.Catalog;

public class CatalogUnitOfWork : UnitOfWork, ICatalogUnitOfWork {

    private readonly IPublisher _publisher;
    private readonly Func<IDbConnection, IDbTransaction, IProductRepository> _productsFactory;
    private readonly Func<IDbConnection, IDbTransaction, IProductAttributeRepository> _productAttributesFactory;
    private readonly Func<IDbConnection, IDbTransaction, IProductClassRepository> _productClassesFactory;

    public IProductRepository Products { get; private set; }
    public IProductAttributeRepository ProductAttributes { get; private set; }
    public IProductClassRepository ProductClasses { get; private set; }

    public CatalogUnitOfWork(ICatalogConnectionFactory factory, ILogger<UnitOfWork> logger, IPublisher publisher,
                            Func<IDbConnection, IDbTransaction, IProductRepository> productsFactory,
                            Func<IDbConnection, IDbTransaction, IProductAttributeRepository> productAttributesFactory,
                            Func<IDbConnection, IDbTransaction, IProductClassRepository> productClassesFactory) : base(factory, logger) {
        _publisher = publisher;
        _productsFactory = productsFactory;
        _productAttributesFactory = productAttributesFactory;
        _productClassesFactory = productClassesFactory;

        Products = _productsFactory(Connection, Transaction);
        ProductAttributes = _productAttributesFactory(Connection, Transaction);
        ProductClasses = _productClassesFactory(Connection, Transaction);
    }

    public override void ResetRepositories() {
        base.ResetRepositories();
        Products = _productsFactory(Connection, Transaction);
        ProductAttributes = _productAttributesFactory(Connection, Transaction);
        ProductClasses = _productClassesFactory(Connection, Transaction);
    }

    public override async Task PublishEvents() {
        await Products.PublishEvents(_publisher);
        await ProductAttributes.PublishEvents(_publisher);
        await ProductClasses.PublishEvents(_publisher);
    }

}