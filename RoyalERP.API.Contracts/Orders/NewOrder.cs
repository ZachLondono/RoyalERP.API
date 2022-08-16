namespace RoyalERP.Contracts.Orders;

public class NewOrder {

    public string Number { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    // Must define either a customer id or name
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }

    // Must define either a vendor id or name
    public Guid? VendorId { get; set; }
    public string? VendorName { get; set; }

}
