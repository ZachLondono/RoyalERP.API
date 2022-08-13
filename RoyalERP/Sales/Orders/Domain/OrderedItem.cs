using RoyalERP.Common.Domain;

namespace RoyalERP.Sales.Orders.Domain;

public class OrderedItem : Entity {

    public Guid OrderId { get; init; }

    public string ProductName { get; init; }

    public int Quantity { get; init; }

    public Dictionary<string, string> Properties { get; init; }

    public OrderedItem(Guid id, Guid orderId, string productName, int quantity, Dictionary<string, string> properties) : base(id) {
        OrderId = orderId;
        ProductName = productName;
        Quantity = quantity;
        Properties = properties;
    }

    private OrderedItem(Guid orderId, string productName, int quantity, Dictionary<string, string> properties) : this(Guid.NewGuid(), orderId, productName, quantity, properties) {
        AddEvent(new Events.OrderedItemCreated(orderId, Id, productName, quantity, properties));
    }

    public static OrderedItem Create(Guid orderId, string productName, int quantity, Dictionary<string, string> properties) => new(orderId, productName, quantity, properties);

}
