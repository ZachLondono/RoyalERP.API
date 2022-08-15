namespace RoyalERP.Contracts.Orders;

public class OrderSearchResult {

    public Guid Id { get; set; }

    public string Number { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateTime PlacedDate { get; set; }

}
