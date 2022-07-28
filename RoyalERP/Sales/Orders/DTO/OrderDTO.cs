using RoyalERP.Sales.Orders.Domain;

namespace RoyalERP.Sales.Orders.DTO;

public class OrderDTO {

    public Guid Id { get; set; }

    public string Number { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateTime PlacedDate { get; set; }

    public DateTime? ConfirmedDate { get; set; } = null;

    public DateTime? CompletedDate { get; set; } = null;

    public OrderStatus Status { get; set; }

}
