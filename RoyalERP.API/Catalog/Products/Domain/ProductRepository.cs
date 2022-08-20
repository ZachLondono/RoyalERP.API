using MediatR;
using RoyalERP.API.Common.Data;
using System.Data;
using RoyalERP.API.Catalog.Products.Data;

namespace RoyalERP.API.Catalog.Products.Domain;

public class ProductRepository : IProductRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<Product> _activeEntities = new();

    public ProductRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddAsync(Product entity) {

        _activeEntities.Add(entity);

        const string command = "INSERT INTO catalog.products (id, name, attributeids) VALUES (@Id, @Name, @AttributeIds);";

        await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new {
            entity.Id,
            entity.Name,
            entity.AttributeIds
        });

    }

    public async Task<IEnumerable<Product>> GetAllAsync() {
        
        const string query = "SELECT id, version, name, classid, attributeids FROM catalog.products";

        var productsData = await _connection.QueryAsync<ProductData>(sql: query, transaction: _transaction);

        var products = new List<Product>();

        foreach (var productData in productsData) { 
            products.Add(new Product(productData.Id, productData.Version, productData.Name, productData.ClassId, new(productData.AttributeIds)));
        }

        return products;
    }

    public async Task<Product?> GetAsync(Guid id) {

        const string query = "SELECT id, version, name, classid, attributeids FROM catalog.products WHERE id = @Id;";

        var productData = await _connection.QuerySingleOrDefaultAsync<ProductData>(sql: query, transaction: _transaction, param: new { Id = id });

        if (productData is null) return null;

        return new Product(productData.Id, productData.Version, productData.Name, productData.ClassId, new(productData.AttributeIds));

    }

    public async Task RemoveAsync(Product entity) {

        const string command = "DELETE FROM catalog.products WHERE id = @Id;";

        var result = await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new { entity.Id });

        if (result == 0) {
            // nothing was deleted
        }

    }

    public async Task UpdateAsync(Product entity) {

        foreach (var domainEvent in entity.Events) {

            if (domainEvent is Events.ProductAttributeAdded attributeAdded) {

                const string command = "UPDATE catalog.products SET attributeids = array_append(attributeids, @AttributeId) WHERE id = @Id;";

                await _connection.ExecuteAsync(command, transaction: _transaction, param: new {
                    Id = attributeAdded.AggregateId,
                    attributeAdded.AttributeId
                });

            } else if (domainEvent is Events.ProductAttributeRemoved attributeRemoved) {

                const string command = "UPDATE catalog.products SET attributeids = array_remove(attributeids, @AttributeId) WHERE id = @Id;";

                await _connection.ExecuteAsync(command, transaction: _transaction, param: new {
                    Id = attributeRemoved.AggregateId,
                    attributeRemoved.AttributeId
                });

            } else if (domainEvent is Events.ProductNameUpdated nameUpdated) {

                const string command = "UPDATE catalog.products SET name = @Name WHERE id = @Id;";

                await _connection.ExecuteAsync(command, transaction: _transaction, param: new {
                    Id = nameUpdated.AggregateId,
                    nameUpdated.Name
                });

            }

        }

        var existing = _activeEntities.FirstOrDefault(o => o.Id == entity.Id);
        if (existing is not null) _activeEntities.Remove(existing);
        _activeEntities.Add(entity);

    }

    public async Task PublishEvents(IPublisher publisher) {
        foreach (var entity in _activeEntities) {
            await entity.PublishEvents(publisher);
        }
    }

}
