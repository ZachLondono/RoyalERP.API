using RoyalERP.Common.Domain;
using RoyalERP.API.Manufacturing.WorkOrders.Domain;

namespace RoyalERP.API.Manufacturing;

public interface IManufacturingUnitOfWork : IUnitOfWork {
    
    IWorkOrderRepository WorkOrders { get; }

}