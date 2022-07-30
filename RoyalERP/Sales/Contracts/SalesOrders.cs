using RoyalERP.Sales.Orders.DTO;

namespace RoyalERP.Sales.Contracts;

public static class SalesOrders {

    public delegate Task<OrderDTO?> GetOrderById(Guid Id);

}