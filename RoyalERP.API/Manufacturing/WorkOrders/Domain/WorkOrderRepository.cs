using MediatR;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.API.Manufacturing.WorkOrders.Domain;

public class WorkOrderRepository : IWorkOrderRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<WorkOrder> _activeEntities = new();

    public WorkOrderRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddAsync(WorkOrder entity) {

        const string command = @"INSERT INTO manufacturing.workorders
                                (id, salesorderid, number, name, note, customername, vendorname, productname, quantity, status)
                                VALUES
                                (@Id, @SalesOrderId, @Number, @Name, @Note, @CustomerName, @VendorName, @ProductName, @Quantity, @Status);";

        await _connection.ExecuteAsync(sql: command, transaction: _transaction, param: new {
            entity.Id,
            entity.SalesOrderId,
            entity.Name,
            entity.Number,
            entity.Note,
            entity.CustomerName, 
            entity.VendorName,
            entity.ProductName,
            entity.Quantity,
            Status = entity.Status.ToString()
        });

    }

    public Task<IEnumerable<WorkOrder>> GetAllAsync() {

        const string query = @"SELECT id, version, salesorderid, number, name, note, productName, quantity, customername, vendorname, status, releaseddate, scheduleddate, fulfilleddate
                                FROM manufacturing.workorders;";

        return _connection.QueryAsync<WorkOrder>(query, transaction: _transaction);

    }

    public Task<WorkOrder?> GetAsync(Guid id) {

        const string query = @"SELECT id, version, salesorderid, number, name, note, productName, quantity, customername, vendorname, status, releaseddate, scheduleddate, fulfilleddate
                                FROM manufacturing.workorders
                                WHERE id = @Id;";

        return _connection.QuerySingleOrDefaultAsync<WorkOrder?>(query, transaction: _transaction, param: new { Id = id });

    }

    public Task RemoveAsync(WorkOrder entity) {

        _activeEntities.Remove(entity);

        const string query = "DELETE FROM manufacturing.workorders WHERE id = @Id;";

        return _connection.ExecuteAsync(query, transaction: _transaction, param: new { entity.Id });

    }

    public async Task UpdateAsync(WorkOrder entity) {

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            // TODO: the events should hold the relevant data to update the db, the entity may have been updated since the event occurred

            if (domainEvent is Events.WorkOrderReleasedEvent released) {

                const string command = "UPDATE manufacturing.workorders SET status = @Status, releaseddate = @ReleasedDate WHERE id = @WorkOrderId;";

                await _connection.ExecuteAsync(command,param: new {
                    released.WorkOrderId,
                    entity.ReleasedDate,
                    Status = WorkOrderStatus.InProgress.ToString()
                }, _transaction);

            } else if (domainEvent is Events.WorkOrderFulfilledEvent fulfilled) {

                const string command = "UPDATE manufacturing.workorders SET status = @Status, fulfilleddate = @FulfilledDate WHERE id = @WorkOrderId;";

                await _connection.ExecuteAsync(command, param: new {
                    fulfilled.WorkOrderId,
                    entity.FulfilledDate,
                    Status = WorkOrderStatus.Fulfilled.ToString()
                }, _transaction);

            } else if (domainEvent is Events.WorkOrderScheduledEvent scheduled) {

                const string command = "UPDATE manufacturing.workorders SET scheduleddate = @ScheduledDate WHERE id = @WorkOrderId;";

                await _connection.ExecuteAsync(command, param: new {
                    scheduled.WorkOrderId,
                    scheduled.ScheduledDate
                }, _transaction);


            } else if (domainEvent is Events.WorkOrderCanceledEvent canceled) {

                const string command = "UPDATE manufacturing.workorders SET status = @Status WHERE id = @WorkOrderId;";

                await _connection.ExecuteAsync(command, param: new {
                    canceled.WorkOrderId,
                    Status = WorkOrderStatus.Cancelled.ToString()
                }, _transaction);

            } else if (domainEvent is Events.WorkOrderNoteSet note) {

                const string command = "UPDATE manufacturing.workorders SET note = @Note WHERE id = @WorkOrderId;";

                await _connection.ExecuteAsync(command, param: new {
                    note.WorkOrderId,
                    note.Note,
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