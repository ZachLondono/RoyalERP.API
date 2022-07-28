using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Common.Data;
using RoyalERP.Sales.Companies.DTO;

namespace RoyalERP.Sales.Companies.Queries;

public class GetById {

    public record Query(Guid Id) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly IDbConnectionFactory _connectionFactory;

        public Handler(IDbConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT (id, version, name) FROM companies WHERE id = @Id;";

            var connection = _connectionFactory.CreateConnection();

            var company = await connection.QuerySingleOrDefaultAsync<CompanyDTO>(query, param: new { Id = id });

            return new OkObjectResult(company);

        }

    }

}
