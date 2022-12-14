using Dapper;
using MediatR;
using RoyalERP.API.Common.Data;
using RoyalERP.API.Contracts.Orders;

namespace RoyalERP.API.Sales.Orders.Queries;

public class GetById {

    public record Query(Guid OrderId) : IRequest<OrderDetails?>;

    public class Handler : IRequestHandler<Query, OrderDetails?> {

        private readonly ISalesConnectionFactory _factory;

        public Handler(ISalesConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<OrderDetails?> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, version, number, name, customerid, vendorid, placeddate, confirmeddate, completeddate, status FROM sales.orders WHERE id = @Id;";
            const string itemQuery = "SELECT id, productid, productname, quantity, properties FROM sales.ordereditems WHERE orderid = @OrderId;";

            var connection = _factory.CreateConnection();

            var order = await connection.QuerySingleOrDefaultAsync<OrderDetails?>(query, new { Id = request.OrderId });

            if (order is null) return null;

            var itemsData = await connection.QueryAsync<(Guid Id, Guid ProductId, string ProductName, int Quantity, Json<Dictionary<string,string>> Properties)>(itemQuery, new { request.OrderId });
            var items = itemsData.Select(i => new OrderedItemDTO() {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Properties = i.Properties.Value ?? new()
            });

            order.Items = items;

            return order;

        }
    }

}
