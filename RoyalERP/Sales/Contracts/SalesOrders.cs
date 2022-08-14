using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Contracts;

public static class SalesOrders {

    public delegate Task<OrderDetails?> GetOrderById(Guid Id);

}