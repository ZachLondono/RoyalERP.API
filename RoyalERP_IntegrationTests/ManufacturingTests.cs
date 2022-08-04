using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RoyalERP.Common.Data;
using RoyalERP.Manufacturing;
using RoyalERP.Manufacturing.WorkOrders.Commands;
using RoyalERP.Manufacturing.WorkOrders.Domain;
using RoyalERP.Manufacturing.WorkOrders.DTO;
using RoyalERP.Manufacturing.WorkOrders.Queries;
using System;
using System.Data;
using System.Threading;
using Xunit;

namespace RoyalERP_IntegrationTests;

public sealed partial class ManufacturingTests : DbTests {

    private readonly CancellationToken _token;

    public ManufacturingTests() {
        
        CancellationTokenSource source = new CancellationTokenSource();
        _token = source.Token;
    }

    [Fact]
    public void Create_ShouldReturnNewWorkOrder() {

        // Arrange
        var expected = new NewWorkOrder() {
            Name = "Order Name",
            Number = "OT123",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        };

        var handler = new Create.Handler(CreateUOW());
        var request = new Create.Command(expected);

        // Act
        var response = handler.Handle(request, _token).Result;


        // Assert
        response.Should().BeOfType<CreatedResult>();

        var created = response as CreatedResult;

        created!.Value.Should().NotBeNull();
        created!.Value.Should().BeOfType<WorkOrderDTO>();

        var returnedEntity = created.Value as WorkOrderDTO;
        returnedEntity!.Should().NotBeNull();
        returnedEntity.Should().BeEquivalentTo(expected);

        var actual = GetOrder(returnedEntity!.Id);
        actual.Should().BeEquivalentTo(expected);

    }

    [Fact]
    public void Delete_ShouldReturnNotFound_WhenDoesntExist() {

        // Arrange
        var orderId = Guid.NewGuid();
        var handler = new Delete.Handler(CreateUOW());
        var request = new Delete.Command(orderId);

        // Act
        var response = handler.Handle(request, _token).Result;

        // Assert
        response.Should().BeOfType<NotFoundResult>();

    }

    [Fact]
    public void Delete_ShouldReturnNoContent_WhenSuccessful() {

        // Arrange
        var dto = CreateNew();

        var handler = new Delete.Handler(CreateUOW());
        var request = new Delete.Command(dto.Id);

        var getHandler = new GetById.Handler(new ConnFactory(dbcontainer.ConnectionString));
        var getRequest = new GetById.Query(dto.Id);

        // Act
        var response = handler.Handle(request, _token).Result;
        var getResponse = getHandler.Handle(getRequest, _token).Result;

        // Assert
        response.Should().BeOfType<NoContentResult>();
        getResponse.Should().BeOfType<NotFoundResult>();

    }

    [Fact]
    public void Cancel_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var dto = CreateNew();

        var handler = new CancelOrder.Handler(CreateUOW());
        var request = new CancelOrder.Command(dto.Id);

        // Act
        var response = handler.Handle(request, _token).Result;
        var queried = GetOrder(dto.Id);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        queried.Status.Should().Be(WorkOrderStatus.Cancelled);

    }

    [Fact]
    public void Fulfill_ShouldUpdateReturnOk() {

        // Arrange
        var dto = CreateNew();

        var handler = new FulfillOrder.Handler(CreateUOW());
        var request = new FulfillOrder.Command(dto.Id);

        // Act
        var response = handler.Handle(request, _token).Result;
        var queried = GetOrder(dto.Id);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        queried.FulfilledDate.Should().Be(DateTime.Today);
        queried.Status.Should().Be(WorkOrderStatus.Fulfilled);

    }

    [Fact]
    public void Release_ShouldUpdateReturnOk() {

        // Arrange
        var dto = CreateNew();

        var handler = new ReleaseOrder.Handler(CreateUOW());
        var request = new ReleaseOrder.Command(dto.Id);

        // Act
        var response = handler.Handle(request, _token).Result;
        var queried = GetOrder(dto.Id);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        queried.ReleasedDate.Should().Be(DateTime.Today);
        queried.Status.Should().Be(WorkOrderStatus.InProgress);

    }

    [Fact]
    public void Schedule_ShouldUpdateReturnOk() {

        // Arrange
        var dto = CreateNew();
        var scheduledDate = DateTime.Today;
        var handler = new ScheduleOrder.Handler(CreateUOW());
        var request = new ScheduleOrder.Command(dto.Id, scheduledDate);

        // Act
        var response = handler.Handle(request, _token).Result;
        var queried = GetOrder(dto.Id);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        queried.ScheduledDate.Should().Be(scheduledDate);

    }

    private WorkOrderDTO CreateNew() {
        var createHandler = new Create.Handler(CreateUOW());
        var createRequest = new Create.Command(new() { CustomerName = "A", Name = "B", Number = "C", VendorName = "D" });
        var createResponse = createHandler.Handle(createRequest, _token).Result;
        return (((CreatedResult)createResponse).Value as WorkOrderDTO)!;
    }

    private WorkOrderDTO GetOrder(Guid id) {
        var getHandler = new GetById.Handler(new ConnFactory(dbcontainer.ConnectionString));
        var getRequest = new GetById.Query(id);
        var getResponse = getHandler.Handle(getRequest, _token).Result;
        return (((OkObjectResult)getResponse).Value as WorkOrderDTO)!;
    }

    private IManufacturingUnitOfWork CreateUOW() {
        var factory = new ConnFactory(dbcontainer.ConnectionString);
        return new ManufacturingUnitOfWork(factory, new FakePublisher(), (conn, trx) => new WorkOrderRepository(new DapperConnection(conn), trx));
    }

    private class ConnFactory : IManufacturingConnectionFactory {

        private readonly string _connString;
        public ConnFactory(string connString) {
            _connString = connString;
        }

        public IDbConnection CreateConnection() {
            return new NpgsqlConnection(_connString);
        }

    }

}