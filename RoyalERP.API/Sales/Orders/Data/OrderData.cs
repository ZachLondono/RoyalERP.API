using RoyalERP.API.Sales.Orders.Domain;

namespace RoyalERP.API.Sales.Orders.Data;

public class OrderData {

    public Guid Id { get; set; }

    public string Number { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public Guid CustomerId { get; set; } = Guid.Empty;

    public Guid VendorId { get; set; } = Guid.Empty;

    public DateTime PlacedDate { get; set; }

    public DateTime? ConfirmedDate { get; set; } = null;

    public DateTime? CompletedDate { get; set; } = null;

    public OrderStatus Status { get; set; }

}
