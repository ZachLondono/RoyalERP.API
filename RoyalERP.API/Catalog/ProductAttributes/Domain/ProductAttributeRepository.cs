using MediatR;

namespace RoyalERP.API.Catalog.ProductAttributes.Domain;

public class ProductAttributeRepository : IProductAttributeRepository {

    public Task AddAsync(ProductAttribute entity) {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductAttribute>> GetAllAsync() {
        throw new NotImplementedException();
    }

    public Task<ProductAttribute?> GetAsync(Guid id) {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(ProductAttribute entity) {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ProductAttribute entity) {
        throw new NotImplementedException();
    }

    public Task PublishEvents(IPublisher publisher) {
        throw new NotImplementedException();
    }

}