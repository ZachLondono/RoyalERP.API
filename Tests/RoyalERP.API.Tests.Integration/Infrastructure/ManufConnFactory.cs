using Npgsql;
using RoyalERP.API.Manufacturing;
using System.Data;

namespace RoyalERP.API.Tests.Integration.Infrastructure;

public class ManufConnFactory : IManufacturingConnectionFactory {

    private readonly string _connString;
    public ManufConnFactory(string connString) {
        _connString = connString;
    }

    public IDbConnection CreateConnection() {
        return new NpgsqlConnection(_connString);
    }

}
