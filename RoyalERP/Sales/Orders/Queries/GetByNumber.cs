using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Orders.DTO;
using System.Data;

namespace RoyalERP.Sales.Orders.Queries;

public class GetByNumber {

    public record Query(string OrderNumber) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly IDbConnection _connection;

        public Handler(ISalesConnectionFactory salesConnectionFactory) {
            _connection = salesConnectionFactory.CreateConnection();
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, version, number, name, customerid, vendorid, placeddate, confirmeddate, completeddate, status FROM sales.orders WHERE LOWER(number) = @OrderNumber;";

            var orders = await _connection.QueryAsync<OrderSearchResult>(query, new { OrderNumber = request.OrderNumber.ToLower() });

            return new OkObjectResult(orders);

        }

    }

}