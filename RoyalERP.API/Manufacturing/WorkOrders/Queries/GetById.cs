using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Contracts.WorkOrders;

namespace RoyalERP.API.Manufacturing.WorkOrders.Queries;

public class GetById {

    public record Query(Guid OrderId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly IManufacturingConnectionFactory _factory;

        public Handler(IManufacturingConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = @"SELECT id, version, salesorderid, number, name, note, productName, quantity, customername, vendorname, releaseddate, scheduleddate, fulfilleddate, status
                                    FROM manufacturing.workorders
                                    WHERE id = @Id;";

            var connection = _factory.CreateConnection();

            var order = await connection.QuerySingleOrDefaultAsync<WorkOrderDTO?>(query, param: new { Id = request.OrderId });

            if (order is null) return new NotFoundResult();

            return new OkObjectResult(order);

        }
    }

}