using FluentAssertions;
using RoyalERP.Manufacturing.WorkOrders.Domain;
using System;
using Xunit;
using static RoyalERP.Manufacturing.WorkOrders.Domain.Exceptions;

namespace RoyalERP_UnitTests.Manufacturing.WorkOrders;

public class WorkOrderTests {

    [Fact]
    public void Create_Should_InitilizeWorkOrder() {

        // Arrange
        string number = "Order Number";
        string name = "Order Name";

        // Act
        var order = WorkOrder.Create(number, name);

        // Assert
        order.Should().NotBeNull();
        order.Name.Should().Be(name);
        order.Number.Should().Be(number);
        order.Status.Should().Be(WorkOrderStatus.Pending);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderCreatedEvent) x).OrderId == order.Id
                                                && ((Events.WorkOrderCreatedEvent)x).Number == number
                                                && ((Events.WorkOrderCreatedEvent)x).Name == name);

    }

    [Fact]
    public void Release_Should_UpdateState_And_CreateEvent() {

        // Arrange
        string number = "Order Number";
        string name = "Order Name";
        var order = new WorkOrder(Guid.NewGuid(), 0,number, name, WorkOrderStatus.Pending, null, null);

        // Act
        order.Release();

        // Assert
        order.Should().NotBeNull();
        order.ReleasedDate.Should().Be(DateTime.Today);
        order.Status.Should().Be(WorkOrderStatus.InProgress);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderReleasedEvent)x).OrderId == order.Id);

    }

    [Fact]
    public void Fulfill_Should_ReleaseFirstAndUpdateState_And_CreateEvent() {

        // Arrange
        string number = "Order Number";
        string name = "Order Name";
        var order = new WorkOrder(Guid.NewGuid(), 0, number, name, WorkOrderStatus.Pending, null, null);

        // Act
        order.Fulfill();

        // Assert
        order.Should().NotBeNull();
        order.ReleasedDate.Should().Be(DateTime.Today);
        order.FulfilledDate.Should().Be(DateTime.Today);
        order.Status.Should().Be(WorkOrderStatus.Fulfilled);
        order.Events.Should().ContainSingle(x => x is Events.WorkOrderReleasedEvent && ((Events.WorkOrderReleasedEvent)x).OrderId == order.Id);
        order.Events.Should().ContainSingle(x => x is Events.WorkOrderFulfilledEvent && ((Events.WorkOrderFulfilledEvent)x).OrderId == order.Id);

    }

    [Fact]
    public void Fulfill_Should_UpdateState_And_CreateEvent() {

        // Arrange
        string number = "Order Number";
        string name = "Order Name";
        var order = new WorkOrder(Guid.NewGuid(), 0, number, name, WorkOrderStatus.InProgress, DateTime.Today, null);

        // Act
        order.Fulfill();

        // Assert
        order.Should().NotBeNull();
        order.FulfilledDate.Should().Be(DateTime.Today);
        order.Status.Should().Be(WorkOrderStatus.Fulfilled);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderFulfilledEvent)x).OrderId == order.Id);

    }

    [Fact]
    public void Cancel_Should_UpdateState_And_CreateEvent() {

        // Arrange
        string number = "Order Number";
        string name = "Order Name";
        var order = new WorkOrder(Guid.NewGuid(), 0, number, name, WorkOrderStatus.InProgress, DateTime.Today, null);

        // Act
        order.Cancel();

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(WorkOrderStatus.Cancelled);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderCanceledEvent)x).OrderId == order.Id);

    }

    [Fact]
    public void Release_Should_ThrowException_WhenCancled() {

        // Arrange
        string number = "Order Number";
        string name = "Order Name";
        var order = new WorkOrder(Guid.NewGuid(), 0, number, name, WorkOrderStatus.Cancelled, null, null);

        // Act
        static void action(WorkOrder o) => o.Release();

        // Assert
        Assert.Throws<CantUpdateCancelledOrderException>(() => action(order));

    }

    [Fact]
    public void Fulfill_Should_ThrowException_WhenCancled() {

        // Arrange
        string number = "Order Number";
        string name = "Order Name";
        var order = new WorkOrder(Guid.NewGuid(), 0, number, name, WorkOrderStatus.Cancelled, null, null);

        // Act
        static void action(WorkOrder o) => o.Fulfill();

        // Assert
        Assert.Throws<CantUpdateCancelledOrderException>(() => action(order));

    }
}
