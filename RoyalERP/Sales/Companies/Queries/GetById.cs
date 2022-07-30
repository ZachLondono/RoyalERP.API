using Dapper;
using MediatR;
using RoyalERP.Sales.Companies.DTO;

namespace RoyalERP.Sales.Companies.Queries;

public class GetById {

    public record Query(Guid ComapnyId) : IRequest<CompanyDTO?>;

    public class Handler : IRequestHandler<Query, CompanyDTO?> {

        private readonly ISalesConnectionFactory _connectionFactory;

        public Handler(ISalesConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public async Task<CompanyDTO?> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, version, name FROM sales.companies WHERE id = @Id;";

            var connection = _connectionFactory.CreateConnection();

            var company = await connection.QuerySingleOrDefaultAsync<CompanyDTO?>(query, param: new { Id = request.ComapnyId });

            return company;

        }

    }

}
