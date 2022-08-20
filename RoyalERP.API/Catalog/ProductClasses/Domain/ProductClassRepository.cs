using MediatR;
using RoyalERP.API.Catalog.Products.Domain;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.API.Catalog.ProductClasses.Domain;

public class ProductClassRepository : IProductClassRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<ProductClass> _activeEntities = new();

    public ProductClassRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public Task AddAsync(ProductClass entity) {

        const string command = "INSERT INTO catalog.productclasses (id, version, name) VALUES (@Id, @Version, @Name);";

        return _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new {
            entity.Id,
            entity.Version,
            entity.Name
        });

    }

    public Task<IEnumerable<ProductClass>> GetAllAsync() {
        const string command = "SELECT id, version, name FROM catalog.productclasses;";
        return _connection.QueryAsync<ProductClass>(sql: command, transaction: _transaction);
    }

    public Task<ProductClass?> GetAsync(Guid id) {
        const string command = "SELECT id, version, name FROM catalog.productclasses WHERE id = @Id;";
        return _connection.QuerySingleOrDefaultAsync<ProductClass?>(sql: command, transaction: _transaction, param: new { Id = id });
    }

    public Task RemoveAsync(ProductClass entity) {
        _activeEntities.Remove(entity);
        const string command = "DELETE FROM catalog.productclasses WHERE id = @Id;";
        return _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new { Id = entity.Id });
    }

    public Task UpdateAsync(ProductClass entity) {
        const string command = "UPDATE catalog.productclasses SET name = @Name WHERE id = @Id;";
        return _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new { entity.Id, entity.Name });
    }

    public async Task PublishEvents(IPublisher publisher) {
        foreach(var entity in _activeEntities) {
            await entity.PublishEvents(publisher);
        }
    }

}
