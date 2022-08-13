using MediatR;
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

    public Guid CustomerId { get; private set; }

    public Guid VendorId { get; private set; }

    public IReadOnlyCollection<OrderedItem> Items => _items.AsReadOnly();
    private readonly List<OrderedItem> _items;

    public Order(Guid id, int version,
                string number, string name, OrderStatus status, Guid customerId, Guid vendorId, List<OrderedItem> items,
                DateTime placedDate, DateTime? confirmedDate = null, DateTime? completedDate = null)
                : base(id, version) {
        Number = number;
        Name = name;
        _items = items;
        PlacedDate = placedDate;
        ConfirmedDate = confirmedDate;
        CompletedDate = completedDate;
        Status = status;
        CustomerId = customerId;
        VendorId = vendorId;
    }

    private Order(string number, string name, Guid customerId, Guid vendorId) : this(Guid.NewGuid(), 0, number, name, OrderStatus.Unconfirmed, customerId, vendorId, new(), DateTime.Today) {
        AddEvent(new Events.OrderPlacedEvent(Id, number, name));
    }

    public static Order Create(string number, string name, Guid customerId, Guid vendorId) => new(number, name, customerId, vendorId);

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

    public OrderedItem AddItem(string productName, int quantity, Dictionary<string, string> properties) {
        if (Status != OrderStatus.Unconfirmed)
            throw new CantAddToConfirmedOrderException();
        var newItem = OrderedItem.Create(Id, productName, quantity, properties);
        _items.Add(newItem);
        return newItem;
    }

    public bool RemoveItem(OrderedItem item) {
        if (!_items.Remove(item)) return false;
        AddEvent(new Events.OrderedItemRemoved(Id, item.Id));
        return true;
    }

    public override async Task<int> PublishEvents(IPublisher publisher) {
        int published = await base.PublishEvents(publisher);
        foreach (var item in _items) {
            published += await item.PublishEvents(publisher);
        }
        return published;
    }

}