using RoyalERP.API.Common.Domain;
using System.Text.Json.Serialization;

namespace RoyalERP.API.Sales.Orders.Domain;

public static class Events {
   
    public abstract record OrderEvent([property: JsonIgnore] Guid OrderId) : DomainEvent(OrderId);

    public record OrderPlacedEvent(Guid OrderId, string Number, string Name) : OrderEvent(OrderId);

    public record OrderConfirmedEvent(Guid OrderId) : OrderEvent(OrderId);

    public record OrderCompletedEvent(Guid OrderId) : OrderEvent(OrderId);

    public record OrderCanceledEvent(Guid OrderId) : OrderEvent(OrderId);

    public record OrderedItemCreated(Guid OrderId, Guid OrderedItemId, Guid ProductId, string ProductName, int Quantity, Dictionary<string, string> Properties) : OrderEvent(OrderId);

    public record OrderedItemRemoved(Guid OrderId, Guid OrderedItemId) : OrderEvent(OrderId);

}
