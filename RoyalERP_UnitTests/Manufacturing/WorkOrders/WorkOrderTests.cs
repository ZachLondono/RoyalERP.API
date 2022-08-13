using Bogus;
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
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";

        // Act
        var order = WorkOrder.Create(salesOrderId, number, name, "", 0, customername, vendorname);

        // Assert
        order.Should().NotBeNull();
        order.Name.Should().Be(name);
        order.Number.Should().Be(number);
        order.Status.Should().Be(WorkOrderStatus.Pending);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderCreatedEvent) x).WorkOrderId == order.Id
                                                && ((Events.WorkOrderCreatedEvent)x).Number == number
                                                && ((Events.WorkOrderCreatedEvent)x).Name == name);

    }

    [Fact]
    public void Release_Should_UpdateState_And_CreateEvent() {

        // Arrange
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";
        var order = new WorkOrder(Guid.NewGuid(), 0, salesOrderId, number, name, "", "", 0, customername, vendorname, WorkOrderStatus.Pending, null, null, null);

        // Act
        order.Release();

        // Assert
        order.Should().NotBeNull();
        order.ReleasedDate.Should().Be(DateTime.Today);
        order.Status.Should().Be(WorkOrderStatus.InProgress);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderReleasedEvent)x).WorkOrderId == order.Id);

    }

    [Fact]
    public void Schedule_Should_UpdateState_And_CreateEvent() {

        // Arrange
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";
        var order = new WorkOrder(Guid.NewGuid(), 0, salesOrderId, number, name, "", "", 0, customername, vendorname, WorkOrderStatus.InProgress, null, null, null);

        // Act
        order.Schedule(DateTime.Today);

        // Assert
        order.Should().NotBeNull();
        order.ScheduledDate.Should().Be(DateTime.Today);
        order.Status.Should().Be(WorkOrderStatus.InProgress);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderScheduledEvent)x).WorkOrderId == order.Id);

    }

    [Fact]
    public void Fulfill_Should_ReleaseFirstAndUpdateState_And_CreateEvent() {

        // Arrange
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";
        var order = new WorkOrder(Guid.NewGuid(), 0, salesOrderId, number, name, "", "", 0, customername, vendorname, WorkOrderStatus.Pending, null, null, null);

        // Act
        order.Fulfill();

        // Assert
        order.Should().NotBeNull();
        order.ReleasedDate.Should().Be(DateTime.Today);
        order.FulfilledDate.Should().Be(DateTime.Today);
        order.Status.Should().Be(WorkOrderStatus.Fulfilled);
        order.Events.Should().ContainSingle(x => x is Events.WorkOrderReleasedEvent && ((Events.WorkOrderReleasedEvent)x).WorkOrderId == order.Id);
        order.Events.Should().ContainSingle(x => x is Events.WorkOrderFulfilledEvent && ((Events.WorkOrderFulfilledEvent)x).WorkOrderId == order.Id);

    }

    [Fact]
    public void Fulfill_Should_UpdateState_And_CreateEvent() {

        // Arrange
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";
        var order = new WorkOrder(Guid.NewGuid(), 0, salesOrderId, number, name, "", "", 0, customername, vendorname, WorkOrderStatus.InProgress, DateTime.Today, null, null);

        // Act
        order.Fulfill();

        // Assert
        order.Should().NotBeNull();
        order.FulfilledDate.Should().Be(DateTime.Today);
        order.Status.Should().Be(WorkOrderStatus.Fulfilled);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderFulfilledEvent)x).WorkOrderId == order.Id);

    }

    [Fact]
    public void Cancel_Should_UpdateState_And_CreateEvent() {

        // Arrange
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";
        var order = new WorkOrder(Guid.NewGuid(), 0, salesOrderId, number, name, "", "", 0, customername, vendorname, WorkOrderStatus.InProgress, DateTime.Today, null, null);

        // Act
        order.Cancel();

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(WorkOrderStatus.Cancelled);
        order.Events.Should().ContainSingle(x => ((Events.WorkOrderCanceledEvent)x).WorkOrderId == order.Id);

    }

    [Fact]
    public void Release_Should_ThrowException_WhenCancled() {

        // Arrange
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";
        var order = new WorkOrder(Guid.NewGuid(), 0, salesOrderId, number, name, "", "", 0, customername, vendorname, WorkOrderStatus.Cancelled, null, null, null);

        // Act
        static void action(WorkOrder o) => o.Release();

        // Assert
        Assert.Throws<CantUpdateOrderException>(() => action(order));

    }

    [Fact]
    public void Schedule_Should_ThrowException_WhenCancled() {

        // Arrange
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";
        var order = new WorkOrder(Guid.NewGuid(), 0, salesOrderId, number, name, "", "", 0, customername, vendorname, WorkOrderStatus.Cancelled, null, null, null);

        // Act
        static void action(WorkOrder o) => o.Schedule(DateTime.Today);

        // Assert
        Assert.Throws<CantUpdateOrderException>(() => action(order));

    }

    [Fact]
    public void Fulfill_Should_ThrowException_WhenCancled() {

        // Arrange
        Guid salesOrderId = Guid.NewGuid();
        string number = "Order Number";
        string name = "Order Name";
        string customername = "Company A";
        string vendorname = "Company B";
        var order = new WorkOrder(Guid.NewGuid(), 0, salesOrderId, number, name, "", "", 0, customername, vendorname, WorkOrderStatus.Cancelled, null, null, null);

        // Act
        static void action(WorkOrder o) => o.Fulfill();

        // Assert
        Assert.Throws<CantUpdateOrderException>(() => action(order));

    }

    [Fact]
    public void SetNote_ShouldUpdateNote_AndAddEvent() {

        // Arrange
        var order = new WorkOrder(Guid.NewGuid(), 0, Guid.NewGuid(), "", "", "", "", 0, "", "", WorkOrderStatus.Cancelled, null, null, null);
        var note = new Faker().Random.Words();

        // Act
        order.SetNote(note);

        // Assert
        order.Note.Should().Be(note);
        order.Events.Should().ContainSingle(x => x is Events.WorkOrderNoteSet 
                                                && ((Events.WorkOrderNoteSet)x).WorkOrderId == order.Id
                                                && ((Events.WorkOrderNoteSet)x).Note == note);

    }

}
