using MediatR;
using RoyalERP.API.Common.Domain;
using RoyalERP.API.Sales.Companies.Domain;
using RoyalERP.API.Sales.Orders.Domain;
using System.Data;

namespace RoyalERP.API.Sales;

public class SalesUnitOfWork : UnitOfWork, ISalesUnitOfWork {

    private readonly IPublisher _publisher;
    private readonly Func<IDbConnection, IDbTransaction, ICompanyRepository> _companiesFactory;
    private readonly Func<IDbConnection, IDbTransaction, IOrderRepository> _ordersFactory;

    public ICompanyRepository Companies { get; private set; }
    public IOrderRepository Orders { get; private set; }

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

    public override async Task PublishEvents() {
        await Orders.PublishEvents(_publisher);
        await Companies.PublishEvents(_publisher);
    }

}