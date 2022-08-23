using RoyalERP.API.Common.Domain;

namespace RoyalERP.API.Sales.Orders.Domain;

public class OrderedItem : Entity {

    public Guid OrderId { get; init; }

    public string ProductName { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public Dictionary<string, string> Properties { get; init; }

    public OrderedItem(Guid id, Guid orderId, Guid productId, string productName, int quantity, Dictionary<string, string> properties) : base(id) {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Properties = properties;
    }

    private OrderedItem(Guid orderId, Guid productId, string productName, int quantity, Dictionary<string, string> properties) : this(Guid.NewGuid(), orderId, productId, productName, quantity, properties) {
        AddEvent(new Events.OrderedItemCreated(orderId, Id, productId, productName, quantity, properties));
    }

    public static OrderedItem Create(Guid orderId, Guid productId, string productName, int quantity, Dictionary<string, string> properties) => new(orderId, productId, productName, quantity, properties);

}
