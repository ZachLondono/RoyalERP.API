using Dapper;
using MediatR;
using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Orders.Queries;

public class GetById {

    public record Query(Guid OrderId) : IRequest<OrderDetails?>;

    public class Handler : IRequestHandler<Query, OrderDetails?> {

        private readonly ISalesConnectionFactory _factory;

        public Handler(ISalesConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<OrderDetails?> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, version, number, name, customerid, vendorid, placeddate, confirmeddate, completeddate, status FROM sales.orders WHERE id = @Id;";
            const string itemQuery = "SELECT id, orderid, productname, properties FROM sales.orderitems WHERE orderid = @OrderId;";

            var connection = _factory.CreateConnection();

            var order = await connection.QuerySingleOrDefaultAsync<OrderDetails>(query, new { Id = request.OrderId });

            var items = await connection.QueryAsync<OrderedItemDTO>(itemQuery, new { request.OrderId });
            order.Items = items;

            return order;

        }
    }

}
