using RoyalERP.Common.Domain;
using RoyalERP.Sales.Companies.Domain;
using RoyalERP.Sales.Orders.Domain;

namespace RoyalERP.Sales;

public interface ISalesUnitOfWork : IUnitOfWork {
    
    ICompanyRepository Companies { get; }
    IOrderRepository Orders { get; }

}