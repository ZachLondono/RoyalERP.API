using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Orders.Queries;

public class GetById {

    public record Query(Guid OrderId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly ISalesConnectionFactory _factory;

        public Handler(ISalesConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, version, number, name, customerid, vendorid, placeddate, confirmeddate, completeddate, status FROM sales.orders WHERE id = @Id;";

            var connection = _factory.CreateConnection();

            var order = await connection.QuerySingleOrDefaultAsync<OrderDTO>(query, new { Id = request.OrderId });

            if (order is null) return new NotFoundResult();

            return new OkObjectResult(order);

        }
    }

}
