using System.Data;

namespace RoyalERP.API.Tests.Unit.Common;

/// <summary>
/// An implementation of IDbTransaction that does not do anything, for unit tests that require it as a dependency
/// </summary>
internal class FakeTransaction : IDbTransaction
{

    public IDbConnection? Connection { get; set; }

    public IsolationLevel IsolationLevel => IsolationLevel.Unspecified;

    public void Commit() { }

    public void Dispose() { }

    public void Rollback() { }

}
