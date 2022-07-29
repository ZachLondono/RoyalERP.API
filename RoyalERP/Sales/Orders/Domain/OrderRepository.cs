using MediatR;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.Sales.Orders.Domain;

public class OrderRepository : IOrderRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<Order> _activeEntities = new();

    public OrderRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddAsync(Order entity) {

        const string command = "INSERT INTO sales.orders (id, number, name, status, customerid, vendorid) values (@Id, @Number, @Name, @Status, @CustomerId, @VendorId);";
        
        await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new {
            entity.Id,
            entity.Number,
            entity.Name,
            entity.CustomerId,
            entity.VendorId,
            Status = entity.Status.ToString()
        });

    }

    public Task<IEnumerable<Order>> GetAllAsync() {

        const string query = "SELECT id, version, number, name, status, customerid, vendorid, placeddate, confirmeddate, completeddate FROM sales.orders;";

        return _connection.QueryAsync<Order>(query, transaction: _transaction);

    }

    public Task<Order?> GetAsync(Guid id) {

        const string query = "SELECT id, version, number, name, status, customerid, vendorid, placeddate, confirmeddate, completeddate FROM sales.orders WHERE id = @Id;";

        return _connection.QuerySingleOrDefaultAsync<Order?>(query, transaction: _transaction, param: new { Id = id });

    }

    public Task RemoveAsync(Order entity) {

        const string query = "DELETE FROM sales.orders WHERE id = @Id;";

        return _connection.ExecuteAsync(query, transaction: _transaction, param: new { entity.Id });

    }

    public async Task UpdateAsync(Order entity) {
        
        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            if (domainEvent is Events.OrderConfirmedEvent confirmed) {

                const string command = "UPDATE sales.orders SET status = @Status, confirmeddate = @ConfirmedDate WHERE id = @Id;";

                await _connection.ExecuteAsync(command, param: new {
                    entity.Id,
                    entity.ConfirmedDate,
                    Status = entity.Status.ToString()
                }, _transaction);

            } else if (domainEvent is Events.OrderCompletedEvent completed) {

                const string command = "UPDATE sales.orders SET status = @Status, completeddate = @CompletedDate WHERE id = @Id;";

                await _connection.ExecuteAsync(command, param: new {
                    entity.Id,
                    entity.ConfirmedDate,
                    Status = entity.Status.ToString()
                }, _transaction);

            } else if (domainEvent is Events.OrderCanceledEvent canceled) {

                const string command = "UPDATE sales.orders SET status = @Status WHERE id = @Id;";

                await _connection.ExecuteAsync(command, param: new {
                    entity.Id,
                    Status = entity.Status.ToString()
                }, _transaction);

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
        _activeEntities.Clear();
    }

}