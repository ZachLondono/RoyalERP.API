using RoyalERP.API.Contracts.Orders;

namespace RoyalERP.API.Sales.Contracts;

public static class SalesOrders {

    public delegate Task<OrderDetails?> GetOrderById(Guid Id);

}