﻿using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Manufacturing.WorkOrders.DTO;

namespace RoyalERP.Manufacturing.WorkOrders.Queries;

public class GetAll {

    public record Query() : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly IManufacturingConnectionFactory _factory;

        public Handler(IManufacturingConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {
            
            const string query = "SELECT (id, version, number, name, releaseddate, fulfilleddate, status) FROM manufacturing.workorders;";

            var connection = _factory.CreateConnection();

            var orders = await connection.QueryAsync<WorkOrderDTO>(query);

            return new OkObjectResult(orders);

        }
    }

}