using MediatR;
using RoyalERP.Common.Domain;
using RoyalERP.Manufacturing.WorkOrders.Domain;
using System.Data;

namespace RoyalERP.Manufacturing;

public class ManufacturingUnitOfWork : UnitOfWork, IManufacturingUnitOfWork {

    public IWorkOrderRepository WorkOrders { get; private set; }

    private readonly IPublisher _publisher;
    private readonly Func<IDbConnection, IDbTransaction, IWorkOrderRepository> _workOrdersFactory;

    public ManufacturingUnitOfWork(IManufacturingConnectionFactory factory, ILogger<UnitOfWork> logger, IPublisher publisher,
                                    Func<IDbConnection, IDbTransaction, IWorkOrderRepository> workOrdersFactory)
                                    : base(factory, logger) {
        _publisher = publisher;
        _workOrdersFactory = workOrdersFactory;
        WorkOrders = _workOrdersFactory(Connection, Transaction);
    }

    public override void ResetRepositories() {
        WorkOrders = _workOrdersFactory(Connection, Transaction);
    }

    public override Task PublishEvents() {
        return WorkOrders.PublishEvents(_publisher);
    }

}
