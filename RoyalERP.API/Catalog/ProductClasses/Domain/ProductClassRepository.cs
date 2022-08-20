using MediatR;

namespace RoyalERP.API.Catalog.ProductClasses.Domain;

public class ProductClassRepository : IProductClassRepository {

    public Task AddAsync(ProductClass entity) {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductClass>> GetAllAsync() {
        throw new NotImplementedException();
    }

    public Task<ProductClass?> GetAsync(Guid id) {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(ProductClass entity) {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ProductClass entity) {
        throw new NotImplementedException();
    }

    public Task PublishEvents(IPublisher publisher) {
        throw new NotImplementedException();
    }

}
