using RoyalERP.Common.Domain;
using System.Text.Json.Serialization;

namespace RoyalERP.Sales.Orders.Domain;

public static class Events {
   
    public abstract record OrderEvent([property: JsonIgnore] Guid OrderId) : DomainEvent(OrderId);

    public record OrderPlacedEvent(Guid OrderId, string Number, string Name) : OrderEvent(OrderId);

    public record OrderConfirmedEvent(Guid OrderId) : OrderEvent(OrderId);

    public record OrderCompletedEvent(Guid OrderId) : OrderEvent(OrderId);

    public record OrderCanceledEvent(Guid OrderId) : OrderEvent(OrderId);

}
