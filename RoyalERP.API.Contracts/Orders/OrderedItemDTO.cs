namespace RoyalERP.API.Contracts.Orders;

public class OrderedItemDTO {

    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public Dictionary<string, string> Properties { get; set; } = new();

}
