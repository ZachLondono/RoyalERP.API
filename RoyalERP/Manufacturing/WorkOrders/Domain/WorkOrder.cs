using RoyalERP.Common.Domain;
using static RoyalERP.Manufacturing.WorkOrders.Domain.Exceptions;

namespace RoyalERP.Manufacturing.WorkOrders.Domain;

public class WorkOrder : AggregateRoot {

    public string Number { get; private set; }

    public string Name { get; private set; }

    public string CustomerName { get; private set; }

    public string VendorName { get; private set; }

    public DateTime? ReleasedDate { get; private set; }
    
    public DateTime? ScheduledDate { get; private set; }

    public DateTime? FulfilledDate { get; private set; }

    public WorkOrderStatus Status { get; private set; }

    public WorkOrder(Guid id, int version,
                    string number, string name, string customerName, string vendorName,
                    WorkOrderStatus status, DateTime? releasedDate, DateTime? scheduledDate, DateTime? fulfilledDate) : base(id, version) {
        Number = number;
        Name = name;
        CustomerName = customerName;
        VendorName = vendorName;
        Status = status;
        ReleasedDate = releasedDate;
        ScheduledDate = scheduledDate;
        FulfilledDate = fulfilledDate;
    }

    private WorkOrder(string number, string name, string customerName, string vendorName) : this(Guid.NewGuid(), 0, number, name, customerName, vendorName, WorkOrderStatus.Pending, null, null, null) {
        AddEvent(new Events.WorkOrderCreatedEvent(Id, number, name));
    }

    public static WorkOrder Create(string number, string name, string customerName, string vendorName) => new(number, name, customerName, vendorName);

    public void Release() {
        if (Status == WorkOrderStatus.Cancelled || Status == WorkOrderStatus.Fulfilled)
            throw new CantUpdateOrderException();
        if (Status == WorkOrderStatus.InProgress)
            return;
        ReleasedDate = DateTime.Today;
        Status = WorkOrderStatus.InProgress;
        AddEvent(new Events.WorkOrderReleasedEvent(Id));
    }

    public void Schedule(DateTime scheduledDate) {
        if (Status == WorkOrderStatus.Cancelled || Status == WorkOrderStatus.Fulfilled)
            throw new CantUpdateOrderException();
        if (Status == WorkOrderStatus.Pending)
            Release();
        ScheduledDate = scheduledDate;
        AddEvent(new Events.WorkOrderScheduledEvent(Id, scheduledDate));
    }

    public void Fulfill() {
        if (Status == WorkOrderStatus.Cancelled || Status == WorkOrderStatus.Fulfilled)
            throw new CantUpdateOrderException();
        if (Status == WorkOrderStatus.Pending)
            Release();
        FulfilledDate = DateTime.Today;
        Status = WorkOrderStatus.Fulfilled;
        AddEvent(new Events.WorkOrderFulfilledEvent(Id));
    }

    public void Cancel() {
        Status = WorkOrderStatus.Cancelled;
        AddEvent(new Events.WorkOrderCanceledEvent(Id));
    }

}