using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.Common.Domain;

public abstract class UnitOfWork : IUnitOfWork, IDisposable {

    protected readonly IDbConnection Connection;
    private IDbTransaction _transaction;
    protected IDbTransaction Transaction => _transaction;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(IDbConnectionFactory factory, ILogger<UnitOfWork> logger) {
        Connection = factory.CreateConnection();
        Connection.Open();
        _transaction = Connection.BeginTransaction();
        _logger = logger;
    }

    public async Task CommitAsync() {

        try {

            _transaction.Commit();

            try {
                await PublishEvents();
            } catch (Exception ex) {
                _logger.LogError("Failed to publish domain events: {Exception}", ex);
            }

        } catch (Exception ex) {

            _logger.LogError("Failed to commit changes to database: {Exception}", ex);
            _transaction.Rollback();
            throw;

        } finally {

            _transaction.Dispose();
            _transaction = Connection.BeginTransaction();

            ResetRepositories();

        }

    }

    public void Rollback() {

        _transaction.Rollback();

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
