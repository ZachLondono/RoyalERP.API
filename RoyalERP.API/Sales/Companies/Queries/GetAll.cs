using MediatR;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.API.Contracts.Companies;
using RoyalERP.API.Sales.Companies.Data;
using RoyalERP.API.Common.Data;

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

            const string query = @"SELECT sales.companies.id as id, version, name, sales.addresses.id as addressid, line1, line2, city, state, zip
                                FROM sales.companies
                                LEFT JOIN sales.addresses
                                ON sales.companies.id = sales.addresses.companyid;";

            var companies =  await connection.QueryAsync<CompanyDTO, AddressDTO, CompanyDTO>(query, map: (c, a) => new() {
                Id = c.Id,
                Name = c.Name,
                Address = a
            }, splitOn: "addressid");

            foreach (var company in companies) {

                const string defaultsQuery = @"SELECT id, companyid, productid, attributeid, value FROM sales.companydefaults WHERE companyid = @CompanyId;";

                var defaultsData = await connection.QueryAsync<DefaultConfigurationData>(defaultsQuery, param: new { CompanyId = company.Id });

                var defaults = new List<DefaultConfigurationDTO>();
                foreach (var defaultData in defaultsData) {
                    defaults.Add(new() {
                        ProductId = defaultData.ProductId,
                        AttributeId = defaultData.AttributeId,
                        Value = defaultData.Value
                    });
                }

                company.Defaults = defaults;

                const string infoQuery = "SELECT info FROM sales.companies WHERE id = @CompanyId;";
                var info = await connection.QuerySingleOrDefaultAsync<Json<Dictionary<string, string>>>(infoQuery, param: new { CompanyId = company.Id});
                company.Info = info?.Value ?? new();

                const string contactsQuery = @"SELECT id, name, email, phone, roles FROM sales.companycontacts WHERE companyid = @CompanyId;";
                var contacts = await connection.QueryAsync<ContactDTO>(contactsQuery, param: new { CompanyId = company.Id });
                company.Contacts = contacts;

            }

            return new OkObjectResult(companies);

        }

    }

}
