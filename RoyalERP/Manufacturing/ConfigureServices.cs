using RoyalERP.Common.Data;
using RoyalERP.Manufacturing.WorkOrders.Domain;
using System.Data;

namespace RoyalERP.Manufacturing;

public static class ConfigureServices {

    public static IServiceCollection AddManufacturing(this IServiceCollection services) {

        Dapper.SqlMapper.AddTypeMap(typeof(WorkOrderStatus), DbType.String);

        services.AddSingleton<Func<IDbConnection, IDbTransaction, IWorkOrderRepository>>(s => (c, t) => new WorkOrderRepository(new DapperConnection(c), t));

        services.AddTransient<IManufacturingUnitOfWork, ManufacturingUnitOfWork>();

        services.AddTransient<IManufacturingConnectionFactory, NpgsqlManufacturingConnectionFactory>();

        return services;

    }

}
