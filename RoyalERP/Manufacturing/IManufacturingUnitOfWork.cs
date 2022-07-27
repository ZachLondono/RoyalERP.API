using RoyalERP.Common.Domain;
using RoyalERP.Manufacturing.WorkOrders.Domain;

namespace RoyalERP.Manufacturing;

public interface IManufacturingUnitOfWork : IUnitOfWork {
    
    IWorkOrderRepository WorkOrders { get; }

}