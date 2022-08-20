using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalERP.API.Common.Data;
using RoyalERP.API.Manufacturing;
using RoyalERP.API.Sales;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using RoyalERP.API.Catalog;

namespace RoyalERP.API.Tests.Integration.Infrastructure;

public abstract class DbTests : WebApplicationFactory<RoyalERP.Program>, IAsyncLifetime {

    protected readonly TestcontainerDatabase dbcontainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                                                           .WithDatabase(new PostgreSqlTestcontainerConfiguration {
                                                               Database = "db",
                                                               Username = "postgres",
                                                               Password = "postgres",
                                                           })
                                                           .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureTestServices(services => {
            services.RemoveAll(typeof(IDbConnectionFactory));
            services.RemoveAll(typeof(IManufacturingConnectionFactory));
            services.RemoveAll(typeof(ISalesConnectionFactory));
            services.RemoveAll(typeof(ICatalogConnectionFactory));
            services.AddSingleton<IManufacturingConnectionFactory>(_ => new ManufConnFactory(dbcontainer.ConnectionString));
            services.AddSingleton<ISalesConnectionFactory>(_ => new SalesConnFactory(dbcontainer.ConnectionString));
            services.AddSingleton<ICatalogConnectionFactory>(_ => new CatalogConnFactory(dbcontainer.ConnectionString));
        });
    }

    public HttpClient CreateClientWithAuth() {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "A1C33C48-CF3A-4DAB-8EB3-CB76976B5690");
        return client;
    }

    public async Task InitializeAsync() {
        await dbcontainer.StartAsync();
        await TestUtils.CreateTables(dbcontainer.ConnectionString);
    }

    public new Task DisposeAsync() {
        return dbcontainer.DisposeAsync().AsTask();
    }
}