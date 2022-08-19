using Npgsql;
using RoyalERP.API.Sales;
using System.Data;

namespace RoyalERP.API.Tests.Integration.Infrastructure;

public class SalesConnFactory : ISalesConnectionFactory {

    private readonly string _connString;
    public SalesConnFactory(string connString) {
        _connString = connString;
    }

    public IDbConnection CreateConnection() {
        return new NpgsqlConnection(_connString);
    }

}