using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP_IntegrationTests.Infrastructure;

public abstract class DbTests : IAsyncLifetime {

    protected readonly TestcontainerDatabase dbcontainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                                                           .WithDatabase(new PostgreSqlTestcontainerConfiguration {
                                                               Database = "db",
                                                               Username = "postgres",
                                                               Password = "postgres",
                                                           })
                                                           .Build();

    public async Task InitializeAsync() {
        await dbcontainer.StartAsync();
        await TestUtils.CreateTables(dbcontainer.ConnectionString);
    }

    public Task DisposeAsync() {
        return dbcontainer.DisposeAsync().AsTask();
    }

}