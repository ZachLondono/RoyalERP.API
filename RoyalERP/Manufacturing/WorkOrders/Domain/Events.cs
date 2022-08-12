using RoyalERP.Common.Domain;
using System.Text.Json.Serialization;

namespace RoyalERP.Manufacturing.WorkOrders.Domain;

public static class Events {

    public abstract record WorkOrderEvent([property: JsonIgnore] Guid WorkOrderId) : DomainEvent(WorkOrderId);

    public record WorkOrderCreatedEvent(Guid WorkOrderId, string Number, string Name) : WorkOrderEvent(WorkOrderId);

    public record WorkOrderReleasedEvent(Guid WorkOrderId) : WorkOrderEvent(WorkOrderId);

    public record WorkOrderScheduledEvent(Guid WorkOrderId, DateTime ScheduledDate) : WorkOrderEvent(WorkOrderId);

    public record WorkOrderFulfilledEvent(Guid WorkOrderId) : WorkOrderEvent(WorkOrderId);

    public record WorkOrderCanceledEvent(Guid WorkOrderId) : WorkOrderEvent(WorkOrderId);

    public record WorkOrderNoteSet(Guid WorkOrderId, string Note) : WorkOrderEvent(WorkOrderId);

}
