using Dapper;
using MediatR;
using RoyalERP.Contracts.Companies;
using RoyalERP.API.Sales.Companies.Data;

namespace RoyalERP.API.Sales.Companies.Queries;

public class GetById {

    public record Query(Guid CompanyId) : IRequest<CompanyDTO?>;

    public class Handler : IRequestHandler<Query, CompanyDTO?> {

        private readonly ISalesConnectionFactory _connectionFactory;

        public Handler(ISalesConnectionFactory connectionFactory) {
            _connectionFactory = connectionFactory;
        }

        public async Task<CompanyDTO?> Handle(Query request, CancellationToken cancellationToken) {

            const string query = @"SELECT sales.companies.id as id, version, name, contact, email, sales.addresses.id as addressid, line1, line2, city, state, zip
                                FROM sales.companies
                                LEFT JOIN sales.addresses
                                ON sales.companies.id = sales.addresses.companyid
                                WHERE sales.companies.id = @Id;";


            var connection = _connectionFactory.CreateConnection();

            var data = await connection.QuerySingleOrDefaultAsync<CompanyData?>(query, param: new { Id = request.CompanyId });
            if (data is null) return null;

            return new CompanyDTO() {
                Id = data.Id,
                Version = data.Version,
                Name = data.Name,
                Contact = data.Contact,
                Email = data.Email,
                Address = new() {
                    Line1 = data.Line1,
                    Line2 = data.Line2,
                    Line3 = data.Line3,
                    City = data.City,
                    State = data.State,
                    Zip = data.Zip,
                }
            };

        }

    }

}
