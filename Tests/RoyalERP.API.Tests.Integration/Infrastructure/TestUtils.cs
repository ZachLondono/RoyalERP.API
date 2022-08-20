using Npgsql;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RoyalERP.API.Tests.Integration.Infrastructure;

public static class TestUtils {

    public static async Task CreateTables(string connectionString) {

        var directory = TryGetSolutionDirectoryInfo();
        if (directory is null)
            throw new InvalidDataException("Could not find path to db schema");

        var salesSchemaPath = Path.Combine(directory.FullName, "RoyalERP.API", "Sales", "Schema", "SalesSchema.sql");
        var manufSchemaPath = Path.Combine(directory.FullName, "RoyalERP.API", "Manufacturing", "Schema", "ManufacturingSchema.sql");
        var catalogSchemaPath = Path.Combine(directory.FullName, "RoyalERP.API", "Catalog", "Schema", "CatalogSchema.sql");

        using var connection = new NpgsqlConnection(connectionString);

        connection.Open();

        await ExecuteScript(connection, salesSchemaPath);
        await ExecuteScript(connection, manufSchemaPath);
        await ExecuteScript(connection, catalogSchemaPath);

        connection.Close();

    }

    private static async Task ExecuteScript(IDbConnection connection, string path) {

        var script = await File.ReadAllTextAsync(path);

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

    public static DirectoryInfo? TryGetSolutionDirectoryInfo(string? currentPath = null) {
        var directory = new DirectoryInfo(
            currentPath ?? Directory.GetCurrentDirectory());
        while (directory != null && !directory.GetFiles("*.sln").Any()) {
            directory = directory.Parent;
        }
        return directory;
    }

}