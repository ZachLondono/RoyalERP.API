using MediatR;
using RoyalERP.Contracts.Companies;
using RoyalERP.Contracts.Orders;

namespace RoyalERP.Sales;

public class SalesQueries {

    private readonly ISender _sender;

    public SalesQueries(ISender sender) {
        _sender = sender;
    }

    public Task<OrderDetails?> GetByOrderId(Guid id) {
        return _sender.Send(new Orders.Queries.GetById.Query(id));
    }

    public Task<CompanyDTO?> GetByCompanyId(Guid id) {
        return _sender.Send(new Companies.Queries.GetById.Query(id));
    }

}