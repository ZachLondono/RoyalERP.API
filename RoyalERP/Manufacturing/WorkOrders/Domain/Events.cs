using RoyalERP.Common.Domain;
using System.Text.Json.Serialization;

namespace RoyalERP.Manufacturing.WorkOrders.Domain;

public static class Events {

    public abstract record WorkOrderEvent([property: JsonIgnore] Guid OrderId) : DomainEvent(OrderId);

    public record WorkOrderCreatedEvent(Guid OrderId, string Number, string Name) : WorkOrderEvent(OrderId);

    public record WorkOrderReleasedEvent(Guid OrderId) : WorkOrderEvent(OrderId);

    public record WorkOrderFulfilledEvent(Guid OrderId) : WorkOrderEvent(OrderId);

    public record WorkOrderCanceledEvent(Guid OrderId) : WorkOrderEvent(OrderId);

}
