using MediatR;
using System.Data.Common;
using Dapper;
using RoyalERP.API.Sales.Orders.Domain;
using RoyalERP.Common.Data;
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

        const string command = "INSERT INTO catalog.products (id, number) VALUES (@Id, @Name);";

        await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new {
            entity.Id,
            entity.Name
        });

        const string attributesCommand = "INSERT INTO catalog.product_attribute (productid, attributeid) VALUES (@ProductId, @AttributeId);";

        foreach (var attributeid in entity.AttributeIds) {

            await _connection.ExecuteAsync(sql: attributesCommand, transaction: _transaction, param: new {
                ProductId = entity.Id,
                AttributeId = attributeid
            });

        }

    }

    public async Task<IEnumerable<Product>> GetAllAsync() {
        
        const string query = "SELECT id, version, name, attributeids FROM catalog.products";

        var productsData = await _connection.QueryAsync<ProductData>(sql: query, transaction: _transaction);

        var products = new List<Product>();

        foreach (var productData in productsData) { 
            products.Add(new Product(productData.Id, productData.Version, productData.Name, new(productData.AttributeIds)));
        }

        return products;
    }

    public async Task<Product?> GetAsync(Guid id) {

        const string query = "SELECT id, version, name, attributeids FROM catalog.products WHERE id = @Id;";

        var productData = await _connection.QuerySingleOrDefaultAsync<ProductData>(sql: query, transaction: _transaction, param: new { Id = id });

        if (productData is null) return null;

        return new Product(productData.Id, productData.Version, productData.Name, new(productData.AttributeIds));

    }

    public async Task PublishEvents(IPublisher publisher) {
        foreach (var entity in _activeEntities) {
            await entity.PublishEvents(publisher);
        }
    }

    public Task RemoveAsync(Product entity) {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Product entity) {
        _activeEntities.Add(entity);
        throw new NotImplementedException();
    }

}
