using MediatR;
using RoyalERP.API.Common.Data;
using RoyalERP.API.Sales.Orders.Data;
using System.Data;

namespace RoyalERP.API.Sales.Orders.Domain;

public class OrderRepository : IOrderRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<Order> _activeEntities = new();

    public OrderRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddAsync(Order entity) {
        
        _activeEntities.Add(entity);

        const string command = "INSERT INTO sales.orders (id, number, name, status, customerid, vendorid, placeddate) values (@Id, @Number, @Name, @Status, @CustomerId, @VendorId, @PlacedDate);";
        
        await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new {
            entity.Id,
            entity.Number,
            entity.Name,
            entity.CustomerId,
            entity.VendorId,
            entity.PlacedDate,
            Status = entity.Status.ToString()
        });

    }

    public Task<IEnumerable<Order>> GetAllAsync() {

        // TODO: get ordered items for each order

        const string query = "SELECT id, version, number, name, status, customerid, vendorid, placeddate, confirmeddate, completeddate FROM sales.orders;";

        return _connection.QueryAsync<Order>(query, transaction: _transaction);

    }

    public async Task<Order?> GetAsync(Guid id) {

        const string query = "SELECT id, version, number, name, status, customerid, vendorid, placeddate, confirmeddate, completeddate FROM sales.orders WHERE id = @Id;";
        const string itemQuery = "SELECT id, orderid, productid, productname, quantity, properties FROM sales.ordereditems WHERE orderid = @OrderId;";

        var order = await _connection.QuerySingleOrDefaultAsync<OrderData?>(query, transaction: _transaction, param: new { Id = id });

        if (order is null) return null;

        var itemsData = await _connection.QueryAsync<(Guid Id, Guid OrderId, Guid ProductId, string ProductName, int Quantity, Json<Dictionary<string,string>> Properties)>(itemQuery, transaction: _transaction, param: new { OrderId = order.Id });

        var items = itemsData.Select(i => new OrderedItem(i.Id, i.OrderId, i.ProductId, i.ProductName, i.Quantity, i.Properties.Value ?? new()));

        return new Order(order.Id, 0, order.Number, order.Name, order.Status, order.CustomerId, order.VendorId, new(items), order.PlacedDate, order.ConfirmedDate, order.CompletedDate);

    }

    public Task RemoveAsync(Order entity) {

        const string query = "DELETE FROM sales.orders WHERE id = @Id;";

        return _connection.ExecuteAsync(query, transaction: _transaction, param: new { entity.Id });

    }

    public async Task UpdateAsync(Order entity) {

        foreach (var item in entity.Items) {
            await UpdateOrderedItem(item);
        }

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            // TODO: the events should hold the relevant data to update the db, the entity may have been updated since the event occurred
            // TODO: use switch 

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
                    entity.CompletedDate,
                    Status = entity.Status.ToString()
                }, _transaction);

            } else if (domainEvent is Events.OrderCanceledEvent canceled) {

                const string command = "UPDATE sales.orders SET status = @Status WHERE id = @Id;";

                await _connection.ExecuteAsync(command, param: new {
                    entity.Id,
                    Status = entity.Status.ToString()
                }, _transaction);

            } else if (domainEvent is Events.OrderedItemRemoved itemRemoved) {

                const string command = "DELETE sales.ordereditems WHERE id = @Id";

                await _connection.ExecuteAsync(command, param: new {
                    Id = itemRemoved.OrderedItemId
                }, _transaction);

            }

        }

        var existing = _activeEntities.FirstOrDefault(o => o.Id == entity.Id);
        if (existing is not null) _activeEntities.Remove(existing);
        _activeEntities.Add(entity);

    }

    private async Task UpdateOrderedItem(OrderedItem item) {

        foreach (var domainEvent in item.Events.Where(e => !e.IsPublished)) {

            switch (domainEvent) {
                case Events.OrderedItemCreated created:

                    const string command = "INSERT INTO sales.ordereditems (id, orderid, productid, productname, quantity, properties)  VALUES (@Id, @OrderId, @ProductId, @ProductName, @Quantity, @Properties);";

                    int rows = await _connection.ExecuteAsync(command, param: new {
                        Id = created.OrderedItemId,
                        created.OrderId,
                        created.ProductId,
                        created.ProductName,
                        created.Quantity,
                        Properties = new JsonParameter(created.Properties)
                    }, _transaction);

                    break;
                default:
                    // TODO: log unknown event
                    break;
            }

        }

    }

    public async Task PublishEvents(IPublisher publisher) {
        foreach (var entity in _activeEntities) {
            await entity.PublishEvents(publisher);
        }
        _activeEntities.Clear();
    }

}