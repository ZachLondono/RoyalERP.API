using Dapper;
using MediatR;

namespace RoyalERP.API.Catalog.Products.Queries;

public class GetProductClass {

    public record Query(Guid ProductId) : IRequest<Guid?>;

    public class Handler : IRequestHandler<Query, Guid?> {

        private readonly ICatalogConnectionFactory _factory;

        public Handler(ICatalogConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<Guid?> Handle(Query request, CancellationToken cancellationToken) {

            var connection = _factory.CreateConnection();

            const string query = "SELECT classid FROM catalog.products WHERE id = @Id;";

            Guid? classId = await connection.QuerySingleOrDefaultAsync<Guid?>(sql: query, param: new {
                Id = request.ProductId
            });

            return classId;

        }
    }

}