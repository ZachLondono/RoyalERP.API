namespace RoyalERP.API.Catalog;

public static class ConfigureServices {

    public static IServiceCollection AddCatalog(this IServiceCollection services) {

        services.AddTransient<ICatalogConnectionFactory, NpgsqlCatalogConnectionFactory>();

        return services;

    }
}
