using RoyalERP.Common.Domain;
using RoyalERP.API.Sales.Companies.Domain;
using RoyalERP.API.Sales.Orders.Domain;

namespace RoyalERP.API.Sales;

public interface ISalesUnitOfWork : IUnitOfWork {
    
    ICompanyRepository Companies { get; }
    IOrderRepository Orders { get; }

}