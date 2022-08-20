using Npgsql;
using RoyalERP.API.Catalog;
using System.Data;

namespace RoyalERP.API.Tests.Integration.Infrastructure;

public class CatalogConnFactory : ICatalogConnectionFactory {

    private readonly string _connString;
    public CatalogConnFactory(string connString) {
        _connString = connString;
    }

    public IDbConnection CreateConnection() {
        return new NpgsqlConnection(_connString);
    }

}