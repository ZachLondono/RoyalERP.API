using MediatR;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.API.Catalog.ProductAttributes.Domain;

public class ProductAttributeRepository : IProductAttributeRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<ProductAttribute> _activeEntities = new();

    public ProductAttributeRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public Task AddAsync(ProductAttribute entity) {

        const string command = "INSERT INTO catalog.productattributes (id, version, name) VALUES (@Id, @Version, @Name);";

        return _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new {
            entity.Id,
            entity.Version,
            entity.Name
        });

    }

    public Task<IEnumerable<ProductAttribute>> GetAllAsync() {
        const string command = "SELECT id, version, name FROM catalog.productattributes;";
        return _connection.QueryAsync<ProductAttribute>(sql: command, transaction: _transaction);
    }

    public Task<ProductAttribute?> GetAsync(Guid id) {
        const string command = "SELECT id, version, name FROM catalog.productattributes WHERE id = @Id;";
        return _connection.QuerySingleOrDefaultAsync<ProductAttribute?>(sql: command, transaction: _transaction, param: new { Id = id });
    }

    public Task RemoveAsync(ProductAttribute entity) {
        _activeEntities.Remove(entity);
        const string command = "DELETE FROM catalog.productattributes WHERE id = @Id;";
        return _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new { Id = entity.Id });
    }

    public Task UpdateAsync(ProductAttribute entity) {
        const string command = "UPDATE catalog.productattributes SET name = @Name WHERE id = @Id;";
        return _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new { entity.Id,entity.Name });
    }

    public async Task PublishEvents(IPublisher publisher) {
        foreach (var entity in _activeEntities) {
            await entity.PublishEvents(publisher);
        }
    }

}