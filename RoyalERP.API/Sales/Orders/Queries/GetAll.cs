using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Orders;

namespace RoyalERP.API.Sales.Orders.Queries;

public class GetAll {

    public record Query() : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly ISalesConnectionFactory _factory;

        public Handler(ISalesConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, version, number, name, customerid, vendorid, placeddate, confirmeddate, completeddate, status FROM sales.orders;";

            var connection = _factory.CreateConnection();

            var orders = await connection.QueryAsync<OrderSummary>(query);

            return new OkObjectResult(orders);

        }
    }

}
