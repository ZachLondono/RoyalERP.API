using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Products;

namespace RoyalERP.API.Catalog.Products.Queries;

public class GetById {

    public record Query(Guid ProductId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly ICatalogConnectionFactory _factory;

        public Handler(ICatalogConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            var connection = _factory.CreateConnection();

            const string query = "SELECT id, version, name, classid, attributeids FROM catalog.products WHERE id = @Id;";

            var product = await connection.QuerySingleOrDefaultAsync<ProductDTO>(sql: query, param: new {
                Id = request.ProductId,
            });

            if (product is null) return new NotFoundResult();

            return new OkObjectResult(product);

        }
    }

}
