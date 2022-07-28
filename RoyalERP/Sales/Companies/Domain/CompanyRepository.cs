using MediatR;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.Sales.Companies.Domain;

public class CompanyRepository : ICompanyRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    public CompanyRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddAsync(Company entity) {

        const string command = "INSERT INTO sales.companies (id, name) values (@Id, @Name);";

        await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: entity);

    }

    public Task<IEnumerable<Company>> GetAllAsync() {

        const string query = "SELECT (id, version, name) FROM sales.companies;";

        return _connection.QueryAsync<Company>(query, transaction: _transaction);

    }

    public Task<Company?> GetAsync(Guid id) {

        const string query = "SELECT (id, version, name) FROM sales.companies WHERE id = @Id;";

        return _connection.QuerySingleOrDefaultAsync<Company?>(query, transaction: _transaction, param: new { Id = id });

    }

    public Task RemoveAsync(Company entity) {

        const string query = "DELETE FROM sales.companies WHERE id = @Id;";

        return _connection.ExecuteAsync(query, transaction: _transaction, param: new { entity.Id });

    }

    public Task UpdateAsync(Company entity) {
        return Task.CompletedTask;
    }
    
    public Task PublishEvents(IPublisher publisher) {
        return Task.CompletedTask;
    }

}