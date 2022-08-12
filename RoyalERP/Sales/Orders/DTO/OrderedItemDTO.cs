namespace RoyalERP.Sales.Orders.DTO;

public class OrderedItemDTO {

    public Guid Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public Dictionary<string, string> Properties { get; set; } = new();

}
