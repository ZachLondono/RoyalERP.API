using RoyalERP.Common.Domain;
using static RoyalERP.Sales.Orders.Domain.Exceptions;

namespace RoyalERP.Sales.Orders.Domain;

public class Order : AggregateRoot {
    
    public string Number { get; private set; }

    public string Name { get; private set; }

    public DateTime PlacedDate { get; private set; }

    public DateTime? ConfirmedDate { get; private set; }

    public DateTime? CompletedDate { get; private set; }

    public OrderStatus Status { get; private set; }

    public Order(Guid id, int version,
                string number, string name, OrderStatus status, DateTime placedDate,
                DateTime? confirmedDate = null, DateTime? completedDate = null) : base(id, version) {
        Number = number;
        Name = name;
        PlacedDate = placedDate;
        ConfirmedDate = confirmedDate;
        CompletedDate = completedDate;
        Status = status;
    }

    private Order(string number, string name) : this(Guid.NewGuid(), 0, number, name, OrderStatus.Unconfirmed, DateTime.Today) {
        AddEvent(new Events.OrderPlacedEvent(Id, number, name));
    }

    public static Order Create(string number, string name) => new(number, name);

    public void Confirm() {
        if (Status == OrderStatus.Confirmed || Status == OrderStatus.Completed)
            return;
        if (Status == OrderStatus.Cancelled)
            throw new CantUpdateCancelledOrderException();
        ConfirmedDate = DateTime.Today;
        Status = OrderStatus.Confirmed;
        AddEvent(new Events.OrderConfirmedEvent(Id));
    }

    public void Complete() {
        if (Status == OrderStatus.Completed)
            return;
        if (Status == OrderStatus.Cancelled)
            throw new CantUpdateCancelledOrderException();
        if (Status == OrderStatus.Unconfirmed)
            Confirm();
        CompletedDate = DateTime.Today;
        Status = OrderStatus.Completed;
        AddEvent(new Events.OrderCompletedEvent(Id));
    }

    public void Cancel() {
        Status = OrderStatus.Cancelled;
        AddEvent(new Events.OrderCanceledEvent(Id));
    }

}