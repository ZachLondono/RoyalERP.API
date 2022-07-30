using Dapper;
using MediatR;
using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Orders.Queries;

public class GetById {

    public record Query(Guid OrderId) : IRequest<OrderDTO?>;

    public class Handler : IRequestHandler<Query, OrderDTO?> {

        private readonly ISalesConnectionFactory _factory;

        public Handler(ISalesConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<OrderDTO?> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, version, number, name, customerid, vendorid, placeddate, confirmeddate, completeddate, status FROM sales.orders WHERE id = @Id;";

            var connection = _factory.CreateConnection();

            var order = await connection.QuerySingleOrDefaultAsync<OrderDTO>(query, new { Id = request.OrderId });

            return order;

        }
    }

}
