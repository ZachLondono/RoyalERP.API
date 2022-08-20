using MediatR;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Companies;

namespace RoyalERP.API.Sales.Companies.Queries;

public class GetAll {

    public record Query() : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {
        
        private readonly ISalesConnectionFactory _connectionFactory;

        public Handler(ISalesConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            var connection = _connectionFactory.CreateConnection();

            const string query = @"SELECT sales.companies.id as id, version, name, contact, email, sales.addresses.id as addressid, line1, line2, city, state, zip
                                FROM sales.companies
                                LEFT JOIN sales.addresses
                                ON sales.companies.id = sales.addresses.companyid;";

            var companies =  await connection.QueryAsync<CompanyDTO, AddressDTO, CompanyDTO>(query, map: (c, a) => new() {
                Id = c.Id,
                Version = c.Version,
                Name = c.Name,
                Contact = c.Contact,
                Email = c.Email,
                Address = a
            }, splitOn: "addressid");

            return new OkObjectResult(companies);

        }

    }

}
