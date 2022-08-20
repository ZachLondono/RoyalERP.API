using MediatR;
using RoyalERP.API.Contracts.Companies;
using RoyalERP.API.Contracts.Orders;

namespace RoyalERP.API.Sales;

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