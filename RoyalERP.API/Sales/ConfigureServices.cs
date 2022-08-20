using RoyalERP.API.Common.Data;
using RoyalERP.API.Sales.Companies.Domain;
using RoyalERP.API.Sales.Orders.Domain;
using RoyalERP.API.Sales.Contracts;
using System.Data;

namespace RoyalERP.API.Sales;

public static class ConfigureServices {

    public static IServiceCollection AddSales(this IServiceCollection services) {

        Dapper.SqlMapper.AddTypeMap(typeof(OrderStatus), DbType.String);

        services.AddSingleton<Func<IDbConnection, IDbTransaction, IOrderRepository>>(s => (c, t) => new OrderRepository(new DapperConnection(c), t));
        services.AddSingleton<Func<IDbConnection, IDbTransaction, ICompanyRepository>>(s => (c, t) => new CompanyRepository(new DapperConnection(c), t));

        services.AddTransient<ISalesUnitOfWork, SalesUnitOfWork>();

        services.AddTransient<ISalesConnectionFactory, NpgsqlSalesConnectionFactory>();

        services.AddSingleton<SalesQueries>();
        services.AddTransient<SalesOrders.GetOrderById>(s => s.GetRequiredService<SalesQueries>().GetByOrderId);
        services.AddTransient<SalesCompanies.GetCompanyById>(s => s.GetRequiredService<SalesQueries>().GetByCompanyId);

        return services;

    }

}
