using MediatR;
using RoyalERP.Common.Domain;
using RoyalERP.Sales.Companies.Domain;
using RoyalERP.Sales.Orders.Domain;
using System.Data;

namespace RoyalERP.Sales;

public class SalesUnitOfWork : UnitOfWork, ISalesUnitOfWork {

    private IPublisher _publisher;
    private readonly Func<IDbConnection, IDbTransaction, ICompanyRepository> _companiesFactory;
    private readonly Func<IDbConnection, IDbTransaction, IOrderRepository> _ordersFactory;

    public ICompanyRepository Companies { get; set; }
    public IOrderRepository Orders { get; set; }

    public SalesUnitOfWork(ISalesConnectionFactory factory, ILogger<UnitOfWork> logger, IPublisher publisher,
                            Func<IDbConnection, IDbTransaction, ICompanyRepository> companiesFactory,
                            Func<IDbConnection, IDbTransaction, IOrderRepository> ordersFactory)
                            : base(factory, logger) {
        _companiesFactory = companiesFactory;
        _publisher = publisher;
        _ordersFactory = ordersFactory;

        Companies = _companiesFactory(Connection, Transaction);
        Orders = _ordersFactory(Connection, Transaction);
    }

    public override void ResetRepositories() {
        base.ResetRepositories();
        Companies = _companiesFactory(Connection, Transaction);
        Orders = _ordersFactory(Connection, Transaction);
    }

    public override Task PublishEvents() {
        return Orders.PublishEvents(_publisher);
    }

}