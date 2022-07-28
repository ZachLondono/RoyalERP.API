using MediatR;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Common.Data;
using RoyalERP.Sales.Companies.DTO;

namespace RoyalERP.Sales.Companies.Queries;

public class GetAll {

    public record Query() : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {
        
        private readonly IDbConnectionFactory _connectionFactory;

        public Handler(IDbConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT (id, version, name) FROM sales.companies;";

            var connection = _connectionFactory.CreateConnection();

            var companies = await connection.QueryAsync<CompanyDTO>(query);

            return new OkObjectResult(companies);

        }

    }

}
