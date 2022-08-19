using Npgsql;
using System.Data;

namespace RoyalERP.API.Catalog;

public class NpgsqlCatalogConnectionFactory : ICatalogConnectionFactory {
    
    private readonly IConfiguration _config;

    public NpgsqlCatalogConnectionFactory(IConfiguration config) {
        _config = config;
    }

    public IDbConnection CreateConnection() {
        string connectionString = _config.GetConnectionString("CatalogDatabase");
        return new NpgsqlConnection(connectionString);
    }

}
