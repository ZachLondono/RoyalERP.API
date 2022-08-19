using MediatR;
using RoyalERP.API.Catalog.Products.Domain;
using RoyalERP.Common.Data;
using RoyalERP.Common.Domain;
using System.Data;

namespace RoyalERP.API.Catalog;

public class CatalogUnitOfWork : UnitOfWork, ICatalogUnitOfWork {

    private readonly IPublisher _publisher;
    private readonly Func<IDbConnection, IDbTransaction, IProductRepository> _prodcutsFactory;

    public IProductRepository Products { get; private set; }

    public CatalogUnitOfWork(IDbConnectionFactory factory, ILogger<UnitOfWork> logger, IPublisher publisher,
                            Func<IDbConnection, IDbTransaction, IProductRepository> prodcutsFactory) : base(factory, logger) {
        _publisher = publisher;
        _prodcutsFactory = prodcutsFactory;

        ResetRepositories();
    }

    public override void ResetRepositories() {
        base.ResetRepositories();
        Products = _prodcutsFactory(Connection, Transaction);
    }

    public override Task PublishEvents() {
        return Products.PublishEvents(_publisher);
    }

}