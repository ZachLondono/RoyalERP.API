using RoyalERP.Common.Domain;
using static RoyalERP.API.Manufacturing.WorkOrders.Domain.Exceptions;

namespace RoyalERP.API.Manufacturing.WorkOrders.Domain;

public class WorkOrder : AggregateRoot {

    public Guid SalesOrderId { get; init; }

    public string Number { get; init; }

    public string Name { get; init; }

    public string CustomerName { get; init; }

    public string VendorName { get; init; }

    public string ProductName { get; init; }

    public int Quantity { get; init; }

    public string Note { get; private set; }

    public DateTime? ReleasedDate { get; private set; }
    
    public DateTime? ScheduledDate { get; private set; }

    public DateTime? FulfilledDate { get; private set; }

    public WorkOrderStatus Status { get; private set; }

    public WorkOrder(Guid id, int version,
                    Guid salesOrderId, string number, string name, string note, string productName, int quantity, string customerName, string vendorName,
                    WorkOrderStatus status, DateTime? releasedDate, DateTime? scheduledDate, DateTime? fulfilledDate)
                    : base(id, version) {
        SalesOrderId = salesOrderId;
        Number = number;
        Name = name;
        CustomerName = customerName;
        VendorName = vendorName;
        ProductName = productName;
        Quantity = quantity;
        Note = note;
        Status = status;
        ReleasedDate = releasedDate;
        ScheduledDate = scheduledDate;
        FulfilledDate = fulfilledDate;
    }

    private WorkOrder(Guid salesOrderId, string number, string name, string productName, int quantity, string customerName, string vendorName)
                    : this(Guid.NewGuid(), 0, salesOrderId, number, name, "", productName, quantity, customerName, vendorName, WorkOrderStatus.Pending, null, null, null) {
        AddEvent(new Events.WorkOrderCreatedEvent(Id, salesOrderId, number, name));
    }

    public static WorkOrder Create(Guid salesOrderId, string number, string name, string productName, int quantity, string customerName, string vendorName)
                            => new(salesOrderId, number, name, productName, quantity, customerName, vendorName);

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

    public void SetNote(string note) {
        Note = note;
        AddEvent(new Events.WorkOrderNoteSet(Id, note));
    }

}