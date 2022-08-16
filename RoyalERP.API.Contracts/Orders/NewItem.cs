namespace RoyalERP.Contracts.Orders;

public class NewItem {

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public Dictionary<string, string> Properties { get; set; } = new();

}
