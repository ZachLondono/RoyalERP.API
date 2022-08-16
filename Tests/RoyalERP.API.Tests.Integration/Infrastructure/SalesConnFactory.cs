using Npgsql;
using RoyalERP.Sales;
using System.Data;

namespace RoyalERP_IntegrationTests.Infrastructure;

public class SalesConnFactory : ISalesConnectionFactory {

    private readonly string _connString;
    public SalesConnFactory(string connString) {
        _connString = connString;
    }

    public IDbConnection CreateConnection() {
        return new NpgsqlConnection(_connString);
    }

}