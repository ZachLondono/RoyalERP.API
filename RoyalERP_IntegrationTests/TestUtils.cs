using Npgsql;
using System.Data;
using System.Threading.Tasks;

namespace RoyalERP_IntegrationTests;

public static class TestUtils {

    private static readonly string _salesSchemaPath = @"C:\Users\Zachary Londono\source\repos\RoyalERP\RoyalERP\Sales\Schema\SalesSchema.sql";
    private static readonly string _manufSchemaPath = @"C:\Users\Zachary Londono\source\repos\RoyalERP\RoyalERP\Manufacturing\Schema\ManufacturingSchema.sql";

    public static async Task CreateTables(string connectionString) {

        using var connection = new NpgsqlConnection(connectionString);

        connection.Open();

        await ExecuteScript(connection, _salesSchemaPath);
        await ExecuteScript(connection, _manufSchemaPath);

        connection.Close();        

    }

    private static async Task ExecuteScript(IDbConnection connection, string path) {

        var script = await System.IO.File.ReadAllTextAsync(path);

        bool closeConnection = false;
        if (connection.State == ConnectionState.Closed) { 
            connection.Open();
            closeConnection = true;
        }

        using var command = connection.CreateCommand();
        command.CommandText = script;
        command.ExecuteNonQuery();

        if (closeConnection) connection.Close();

    }

}