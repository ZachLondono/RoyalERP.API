using MediatR;
using RoyalERP.Manufacturing.WorkOrders.Domain;
using RoyalERP.Sales.Contracts;
using static RoyalERP.Sales.Orders.Domain.Events;

namespace RoyalERP.Manufacturing.WorkOrders;

public class OrderConfirmedHandler : INotificationHandler<OrderConfirmedEvent> {

    private readonly IManufacturingUnitOfWork _work;
    private readonly SalesOrders.GetOrderById _getOrderById;
    private readonly SalesCompanies.GetCompanyById _getCompanyById;

    public OrderConfirmedHandler(IManufacturingUnitOfWork work, SalesOrders.GetOrderById getOrderById, SalesCompanies.GetCompanyById getCompanyById) {
        _work = work;
        _getOrderById = getOrderById;
        _getCompanyById = getCompanyById;
    }

    public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken) {

        var salesorder = await _getOrderById(notification.OrderId);
        
        if (salesorder is null) {
            // Log null order in notification
            return;
        }

        var customer = await _getCompanyById(salesorder.CustomerId);
        var vendor = await _getCompanyById(salesorder.VendorId);

        if (customer is null) {
            // Log null customer in notification
            return;
        }

        if (vendor is null) {
            // Log null vendor in notification
            return;
        }

        var newOrder = WorkOrder.Create(salesorder.Number, salesorder.Name, customer.Name, vendor.Name);

        await _work.WorkOrders.AddAsync(newOrder);

        await _work.CommitAsync();

    }

}
