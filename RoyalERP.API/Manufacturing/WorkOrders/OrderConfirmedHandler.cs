using MediatR;
using RoyalERP.API.Manufacturing.WorkOrders.Commands;
using RoyalERP.API.Sales.Contracts;
using static RoyalERP.API.Sales.Orders.Domain.Events;

namespace RoyalERP.API.Manufacturing.WorkOrders;

public class OrderConfirmedHandler : INotificationHandler<OrderConfirmedEvent> {

    private readonly ISender _sender;
    private readonly SalesOrders.GetOrderById _getOrderById;
    private readonly SalesCompanies.GetCompanyById _getCompanyById;
    private readonly ILogger<OrderConfirmedHandler> _logger;

    public OrderConfirmedHandler(ISender sender, ILogger<OrderConfirmedHandler> logger, SalesOrders.GetOrderById getOrderById, SalesCompanies.GetCompanyById getCompanyById) {
        _sender = sender;
        _getOrderById = getOrderById;
        _getCompanyById = getCompanyById;
        _logger = logger;
    }

    public async Task Handle(OrderConfirmedEvent notification, CancellationToken cancellationToken) {

        var salesorder = await _getOrderById(notification.OrderId);
        
        if (salesorder is null) {
            _logger.LogError("While trying to create work order, could not find order with id: {OrderId}", notification.OrderId);
            return;
        }

        var customer = await _getCompanyById(salesorder.CustomerId);
        var vendor = await _getCompanyById(salesorder.VendorId);

        string customerName = "Unkown Customer";
        string vendorName = "Unkown Vendor";

        if (customer is null) {
            _logger.LogWarning("While trying to create work order, could not find customer with id: {CompanyId}", salesorder.CustomerId);
        } else {
            customerName = customer.Name;
        }

        if (vendor is null) {
            _logger.LogWarning("While trying to create work order, could not find vendor with id: {CompanyId}", salesorder.CustomerId);
        } else {
            vendorName = vendor.Name;
        }

        var productQtyByType = salesorder.Items
                                        .GroupBy(i => i.ProductName)
                                        .ToDictionary(g => g.Key,
                                                      g => g.Sum(i => i.Quantity));

        foreach (var product in productQtyByType) {

            _logger.LogTrace("Creating a new work order for product {ProductName} with quantity {Quantity}", product.Key, product.Value);

            var newOrder = await _sender.Send(new Create.Command(new() {
                SalesOrderId = salesorder.Id,
                ProductName = product.Key,
                Quantity = product.Value,
                Number = salesorder.Number,
                Name = salesorder.Name,
                CustomerName = customerName,
                VendorName = vendorName,
            }), cancellationToken);

            _logger.LogInformation("New Work Order created with id {WorkOrderId}", newOrder.Id);

        }

    }

}
