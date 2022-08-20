using RoyalERP.Common.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RoyalERP.API.Tests.Unit.Common;

/// <summary>
/// An implementation of IDapperConnection that does not do anything, for unit tests that require it as a dependency
/// </summary>
internal class FakeConnection : IDapperConnection
{

    public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => Task.FromResult(1);

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => Task.FromResult(Enumerable.Empty<T>());

    public Task<IEnumerable<T3>> QueryAsync<T1, T2, T3>(string sql, Func<T1, T2, T3> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
        => Task.FromResult(Enumerable.Empty<T3>());

    public Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        => Task.FromResult<T?>(default);

}
