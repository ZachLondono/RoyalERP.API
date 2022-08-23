namespace RoyalERP.API.Contracts.Orders;

public class NewItem {

    public string ProductName { get; set; } = string.Empty;

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public Dictionary<string, string> Properties { get; set; } = new();

}
