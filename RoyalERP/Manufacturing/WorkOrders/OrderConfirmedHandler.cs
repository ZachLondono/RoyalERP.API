using MediatR;
using RoyalERP.Manufacturing.WorkOrders.Commands;
using RoyalERP.Sales.Contracts;
using static RoyalERP.Sales.Orders.Domain.Events;

namespace RoyalERP.Manufacturing.WorkOrders;

public class OrderConfirmedHandler : INotificationHandler<OrderConfirmedEvent> {

    private readonly ISender _sender;
    private readonly SalesOrders.GetOrderById _getOrderById;
    private readonly SalesCompanies.GetCompanyById _getCompanyById;

    public OrderConfirmedHandler(ISender sender, SalesOrders.GetOrderById getOrderById, SalesCompanies.GetCompanyById getCompanyById) {
        _sender = sender;
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

        await _sender.Send(new Create.Command(new() {
            Number = salesorder.Number,
            Name = salesorder.Name,
            CustomerName = customer.Name,
            VendorName = vendor.Name,
        }), cancellationToken);

    }

}
