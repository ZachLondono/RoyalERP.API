using Npgsql;
using RoyalERP.Common.Data;
using System.Data;

namespace RoyalERP.Sales;

public class NpgsqlSalesConnectionFactory : IDbConnectionFactory {
    
    private readonly IConfiguration _config;

    public NpgsqlSalesConnectionFactory(IConfiguration config) {
        _config = config;
    }

    public IDbConnection CreateConnection() {
        string connectionString = _config.GetConnectionString("SalesDatabase");
        return new NpgsqlConnection(connectionString);
    }

}