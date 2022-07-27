using FluentAssertions;
using RoyalERP.Sales.Orders.Domain;
using System;
using Xunit;
using static RoyalERP.Sales.Orders.Domain.Exceptions;

namespace RoyalERP_UnitTests.Sales.Orders;

public class OrderTests {

    [Fact]
    public void Create_Should_InitilizeOrderAndCreateEvent() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";

        // Act
        var order = Order.Create(number, name);

        // Assert
        order.Should().NotBeNull();;
        order.Name.Should().Be(name);
        order.Number.Should().Be(number);
        order.Status.Should().Be(OrderStatus.Unconfirmed);
        order.Events.Should().ContainSingle(x => ((Events.OrderPlacedEvent)x).OrderId == order.Id);

    }

    [Fact]
    public void Confirm_Should_UpdateOrderAndCreateEvent() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";
        var order = new Order(Guid.NewGuid(), 0, number, name, OrderStatus.Unconfirmed, DateTime.Today);

        // Act
        order.Confirm();

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.ConfirmedDate.Should().Be(DateTime.Today);
        order.Events.Should().ContainSingle(x => ((Events.OrderConfirmedEvent)x).OrderId == order.Id);

    }

    [Fact]
    public void Confirm_Should_NotUpdateOrderAndCreateEvent_When_AlreadyConfirmed() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";
        var order = new Order(Guid.NewGuid(), 0, number, name, OrderStatus.Confirmed, DateTime.Today);

        // Act
        order.Confirm();

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.Events.Should().BeEmpty();

    }

    [Fact]
    public void Confirm_Should_ThrowException_When_Cancelled() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";
        var order = new Order(Guid.NewGuid(), 0, number, name, OrderStatus.Cancelled, DateTime.Today);

        // Act
        static void confirm(Order o) => o.Confirm();

        // Assert
        Assert.Throws<CantUpdateCancelledOrderException>(() => confirm(order));

    }

    [Fact]
    public void Complete_Should_UpdateOrderAndCreateEvent_WhenConfirmed() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";
        var order = new Order(Guid.NewGuid(), 0, number, name, OrderStatus.Confirmed, DateTime.Today);

        // Act
        order.Complete();

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Completed);
        order.CompletedDate.Should().Be(DateTime.Today);
        order.Events.Should().ContainSingle(x => ((Events.OrderCompletedEvent)x).OrderId == order.Id);

    }

    [Fact]
    public void Complete_Should_UpdateOrderAndCreateEvents_WhenUnconfirmed() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";
        var order = new Order(Guid.NewGuid(), 0, number, name, OrderStatus.Unconfirmed, DateTime.Today);

        // Act
        order.Complete();

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Completed);
        order.CompletedDate.Should().Be(DateTime.Today);
        order.ConfirmedDate.Should().Be(DateTime.Today);
        order.Events.Should().ContainSingle(x => x is Events.OrderCompletedEvent && ((Events.OrderCompletedEvent)x).OrderId == order.Id);
        order.Events.Should().ContainSingle(x => x is Events.OrderConfirmedEvent && ((Events.OrderConfirmedEvent)x).OrderId == order.Id);

    }

    [Fact]
    public void Complete_Should_NotUpdateOrderAndCreateEvent_When_AlreadyCompleted() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";
        var order = new Order(Guid.NewGuid(), 0, number, name, OrderStatus.Completed, DateTime.Today);

        // Act
        order.Complete();

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Completed);
        order.Events.Should().BeEmpty();

    }

    [Fact]
    public void Complete_Should_ThrowException_When_Cancelled() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";
        var order = new Order(Guid.NewGuid(), 0, number, name, OrderStatus.Cancelled, DateTime.Today);

        // Act
        static void complete(Order o) => o.Complete();

        // Assert
        Assert.Throws<CantUpdateCancelledOrderException>(() => complete(order));

    }

    [Fact]
    public void Cancel_Should_UpdateOrderAndCreateEvent() {

        // Arrange
        string number = "Order Number";
        string name = "Company Name";
        var order = new Order(Guid.NewGuid(), 0, number, name, OrderStatus.Completed, DateTime.Today);

        // Act
        order.Cancel();

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.Events.Should().ContainSingle(x => ((Events.OrderCanceledEvent)x).OrderId == order.Id);

    }

}
