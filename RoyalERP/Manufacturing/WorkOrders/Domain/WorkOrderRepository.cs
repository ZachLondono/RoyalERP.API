using MediatR;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.Manufacturing.WorkOrders.Domain;

public class WorkOrderRepository : IWorkOrderRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    public WorkOrderRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public Task AddAsync(WorkOrder entity) {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<WorkOrder>> GetAllAsync() {
        throw new NotImplementedException();
    }

    public Task<WorkOrder?> GetAsync(Guid id) {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(WorkOrder entity) {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(WorkOrder entity) {
        throw new NotImplementedException();
    }

    public Task PublishEvents(IPublisher publisher) {
        throw new NotImplementedException();
    }

}