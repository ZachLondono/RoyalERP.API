using RoyalERP.Common.Domain;
using static RoyalERP.Manufacturing.WorkOrders.Domain.Exceptions;

namespace RoyalERP.Manufacturing.WorkOrders.Domain;

public class WorkOrder : AggregateRoot {

    public string Number { get; private set; }

    public string Name { get; private set; }

    public DateTime? ReleasedDate { get; private set; }

    public DateTime? FulfilledDate { get; private set; }

    public WorkOrderStatus Status { get; private set; }

    public WorkOrder(Guid id, int version,
                    string number, string name,
                    WorkOrderStatus status, DateTime? releasedDate, DateTime? fulfilledDate) : base(id, version) {
        Number = number;
        Name = name;
        Status = status;
        ReleasedDate = releasedDate;
        FulfilledDate = fulfilledDate;
    }

    private WorkOrder(string number, string name) : this(Guid.NewGuid(), 0, number, name, WorkOrderStatus.Pending, null, null) {
        AddEvent(new Events.WorkOrderCreatedEvent(Id, number, name));
    }

    public static WorkOrder Create(string number, string name) => new(number, name);

    public void Release() {
        if (Status == WorkOrderStatus.Cancelled)
            throw new CantUpdateCancelledOrderException();
        if (Status == WorkOrderStatus.InProgress || Status == WorkOrderStatus.Fulfilled)
            return;
        ReleasedDate = DateTime.Today;
        Status = WorkOrderStatus.InProgress;
        AddEvent(new Events.WorkOrderReleasedEvent(Id));
    }

    public void Fulfill() {
        if (Status == WorkOrderStatus.Pending)
            Release();
        else if (Status == WorkOrderStatus.Cancelled || Status == WorkOrderStatus.Fulfilled)
            throw new CantUpdateCancelledOrderException();
        FulfilledDate = DateTime.Today;
        Status = WorkOrderStatus.Fulfilled;
        AddEvent(new Events.WorkOrderFulfilledEvent(Id));
    }

    public void Cancel() {
        Status = WorkOrderStatus.Cancelled;
        AddEvent(new Events.WorkOrderCanceledEvent(Id));
    }

}