using MediatR;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.Sales.Orders.Domain;

public class OrderRepository : IOrderRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    public OrderRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public Task AddAsync(Order entity) {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Order>> GetAllAsync() {
        throw new NotImplementedException();
    }

    public Task<Order?> GetAsync(Guid id) {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Order entity) {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Order entity) {
        throw new NotImplementedException();
    }

    public Task PublishEvents(IPublisher publisher) {
        throw new NotImplementedException();
    }

}