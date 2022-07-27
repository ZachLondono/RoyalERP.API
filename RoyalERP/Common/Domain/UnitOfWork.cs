using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.Common.Domain;

public abstract class UnitOfWork : IUnitOfWork, IDisposable {

    protected readonly IDbConnection Connection;
    private IDbTransaction _transaction;
    protected IDbTransaction Transaction => _transaction;

    public UnitOfWork(IDbConnectionFactory factory) {
        Connection = factory.CreateConnection();
        Connection.Open();
        _transaction = Connection.BeginTransaction();
    }

    public Task CommitAsync() {

        try {

            _transaction.Commit();

            try {
                PublishEvents();
            } catch {
                // TODO log exception
            }

        } catch {

            // TODO log exception
            _transaction.Rollback();
            throw;

        } finally {

            _transaction.Dispose();
            _transaction = Connection.BeginTransaction();

            ResetRepositories();

        }

        return Task.CompletedTask;

    }

    public void Dispose() {
        _transaction.Dispose();
        Connection.Dispose();
        GC.SuppressFinalize(this);
    }

    public virtual void ResetRepositories() {
        return;
    }

    public virtual Task PublishEvents() {
        return Task.CompletedTask;
    }

    /*private Task PublishEvents() {
        // TODO: maybe use reflection to get all IRepositories and call PublishEvents
        var properties = this.GetType()
                            .GetProperties()
                            .Where(x => x.GetType()
                                            .GetInterfaces()
                                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRepository<>))
                            ).ToList();

    }*/

}
