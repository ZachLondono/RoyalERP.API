using MediatR;
using RoyalERP.Common.Data;
using RoyalERP.API.Manufacturing.WorkOrders;
using RoyalERP.API.Manufacturing.WorkOrders.Domain;
using System.Data;
using static RoyalERP.API.Sales.Orders.Domain.Events;

namespace RoyalERP.API.Manufacturing;

public static class ConfigureServices {

    public static IServiceCollection AddManufacturing(this IServiceCollection services) {

        Dapper.SqlMapper.AddTypeMap(typeof(WorkOrderStatus), DbType.String);

        services.AddSingleton<Func<IDbConnection, IDbTransaction, IWorkOrderRepository>>(s => (c, t) => new WorkOrderRepository(new DapperConnection(c), t));

        services.AddTransient<IManufacturingUnitOfWork, ManufacturingUnitOfWork>();

        services.AddTransient<IManufacturingConnectionFactory, NpgsqlManufacturingConnectionFactory>();


        return services;

    }

}
