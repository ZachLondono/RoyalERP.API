using RoyalERP.API.Catalog.ProductAttributes.Domain;
using RoyalERP.API.Catalog.ProductClasses.Domain;
using RoyalERP.API.Catalog.Products.Domain;
using RoyalERP.API.Common.Data;
using RoyalERP.API.Catalog.Contracts;
using System.Data;

namespace RoyalERP.API.Catalog;

public static class ConfigureServices {

    public static IServiceCollection AddCatalog(this IServiceCollection services) {

        services.AddTransient<ICatalogConnectionFactory, NpgsqlCatalogConnectionFactory>();

        services.AddSingleton<Func<IDbConnection, IDbTransaction, IProductRepository>>(s => (c, t) => new ProductRepository(new DapperConnection(c), t));
        services.AddSingleton<Func<IDbConnection, IDbTransaction, IProductClassRepository>>(s => (c, t) => new ProductClassRepository(new DapperConnection(c), t));
        services.AddSingleton<Func<IDbConnection, IDbTransaction, IProductAttributeRepository>>(s => (c, t) => new ProductAttributeRepository(new DapperConnection(c), t));

        services.AddTransient<ICatalogUnitOfWork, CatalogUnitOfWork>();

        services.AddTransient<CatalogQueries>();
        services.AddTransient<ProductCatalog.GetProductClassByProductId>(s => s.GetRequiredService<CatalogQueries>().GetProductClassFromProductId);

        return services;

    }
}
