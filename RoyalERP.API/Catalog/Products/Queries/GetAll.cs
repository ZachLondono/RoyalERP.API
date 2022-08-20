using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Products;

namespace RoyalERP.API.Catalog.Products.Queries;

public class GetAll {

    public record Query() : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly ICatalogConnectionFactory _factory;

        public Handler(ICatalogConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            var connection = _factory.CreateConnection();

            const string query = "SELECT id, version, name, classid, attributeids FROM catalog.products";

            var products = await connection.QueryAsync<ProductDTO>(sql: query);

            return new OkObjectResult(products);

        }
    }

}