using Npgsql;
using System.Data;

namespace RoyalERP.Manufacturing;

public class NpgsqlManufacturingConnectionFactory : IManufacturingConnectionFactory {

    private readonly IConfiguration _config;

    public NpgsqlManufacturingConnectionFactory(IConfiguration config) {
        _config = config;
    }

    public IDbConnection CreateConnection() {
        string connectionString = _config.GetConnectionString("ManufacturingDatabase");
        return new NpgsqlConnection(connectionString);
    }

}