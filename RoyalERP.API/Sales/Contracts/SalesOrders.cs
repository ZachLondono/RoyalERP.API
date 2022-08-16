using RoyalERP.Contracts.Orders;

namespace RoyalERP.Sales.Contracts;

public static class SalesOrders {

    public delegate Task<OrderDetails?> GetOrderById(Guid Id);

}