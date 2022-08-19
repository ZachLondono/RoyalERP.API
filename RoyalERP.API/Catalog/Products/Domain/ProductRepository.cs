using MediatR;

namespace RoyalERP.API.Catalog.Products.Domain;

public class ProductRepository : IProductRepository {

    public Task AddAsync(Product entity) {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetAllAsync() {
        throw new NotImplementedException();
    }

    public Task<Product?> GetAsync(Guid id) {
        throw new NotImplementedException();
    }

    public Task PublishEvents(IPublisher publisher) {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Product entity) {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Product entity) {
        throw new NotImplementedException();
    }

}
