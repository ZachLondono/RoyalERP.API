using Npgsql;
using System.Data;

namespace RoyalERP.API.Sales;

public class NpgsqlSalesConnectionFactory : ISalesConnectionFactory {

    private readonly IConfiguration _config;

    public NpgsqlSalesConnectionFactory(IConfiguration config) {
        _config = config;
    }

    public IDbConnection CreateConnection() {
        string connectionString = _config.GetConnectionString("SalesDatabase");
        return new NpgsqlConnection(connectionString);
    }

}