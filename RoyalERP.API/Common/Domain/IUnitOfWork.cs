namespace RoyalERP.API.Common.Domain;

public interface IUnitOfWork {

    public Task CommitAsync(/* CancellationToken */);
    public void Rollback();

}