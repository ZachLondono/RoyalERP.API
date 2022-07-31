using MediatR;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.Manufacturing.WorkOrders.Domain;

public class WorkOrderRepository : IWorkOrderRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<WorkOrder> _activeEntities = new();

    public WorkOrderRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddAsync(WorkOrder entity) {

        const string command = "INSERT INTO manufacturing.workorders (id, number, name, customername, vendorname, status) values (@Id, @Number, @Name, @CustomerName, @VendorName, @Status);";

        await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new {
            entity.Id,
            entity.Name,
            entity.Number,
            entity.CustomerName, 
            entity.VendorName,
            Status = entity.Status.ToString()
        });

    }

    public Task<IEnumerable<WorkOrder>> GetAllAsync() {

        const string query = "SELECT id, version, number, name, customername, vendorname, releaseddate, scheduleddate, fulfilleddate, status FROM manufacturing.workorders;";

        return _connection.QueryAsync<WorkOrder>(query, transaction: _transaction);

    }

    public Task<WorkOrder?> GetAsync(Guid id) {

        const string query = "SELECT id, version, number, name, customername, vendorname, releaseddate, scheduleddate, fulfilleddate, status FROM manufacturing.workorders WHERE id = @Id;";

        return _connection.QuerySingleOrDefaultAsync<WorkOrder?>(query, transaction: _transaction, param: new { Id = id });

    }

    public Task RemoveAsync(WorkOrder entity) {

        const string query = "DELETE FROM manufacturing.workorders WHERE id = @Id;";

        return _connection.ExecuteAsync(query, transaction: _transaction, param: new { entity.Id });

    }

    public async Task UpdateAsync(WorkOrder entity) {

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            // TODO: the events should hold the relevant data to update the db, the entity may have been updated since the event occurred

            if (domainEvent is Events.WorkOrderReleasedEvent released) {

                const string command = "UPDATE manufacturing.workorders SET status = @Status, releaseddate = @ConfirmedDate WHERE id = @OrderId;";

                await _connection.ExecuteAsync(command, entity, _transaction);

            } else if (domainEvent is Events.WorkOrderFulfilledEvent fulfilled) {

                const string command = "UPDATE manufacturing.workorders SET status = @Status, fulfilleddate = @CompletedDate WHERE id = @Id;";

                await _connection.ExecuteAsync(command, entity, _transaction);

            } else if (domainEvent is Events.WorkOrderCanceledEvent scheduled) {

                const string command = "UPDATE manufacturing.workorders SET scheduleddate = @ScheduledDate WHERE id = @OrderId;";

                await _connection.ExecuteAsync(command, scheduled, _transaction);


            } else if (domainEvent is Events.WorkOrderCanceledEvent canceled) {

                const string command = "UPDATE manufacturing.workorders SET status = @Status WHERE id = @Id;";

                await _connection.ExecuteAsync(command, entity, _transaction);

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