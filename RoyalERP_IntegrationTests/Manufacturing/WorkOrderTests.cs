using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RoyalERP.Common.Data;
using RoyalERP.Common.Domain;
using RoyalERP.Manufacturing;
using RoyalERP.Manufacturing.WorkOrders.Commands;
using RoyalERP.Manufacturing.WorkOrders.Domain;
using RoyalERP.Manufacturing.WorkOrders.DTO;
using RoyalERP.Manufacturing.WorkOrders.Queries;
using RoyalERP_IntegrationTests.Infrastructure;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP_IntegrationTests.Manufacturing;

public class WorkOrderTests : DbTests {

    /*private readonly CancellationToken _token;

    public WorkOrderTests() {

        CancellationTokenSource source = new();
        _token = source.Token;
    }

    [Fact]
    public void Delete_ShouldReturnNotFound_WhenDoesntExist() {

        // Arrange
        var orderId = Guid.NewGuid();
        var handler = new Delete.Handler(CreateUOW(), new FakeLogger<Delete.Handler>());
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

        var handler = new Delete.Handler(CreateUOW(), new FakeLogger<Delete.Handler>());
        var request = new Delete.Command(dto.Id);

        var getHandler = new GetById.Handler(new ManufConnFactory(dbcontainer.ConnectionString));
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

        var handler = new CancelOrder.Handler(CreateUOW(), new FakeLogger<CancelOrder.Handler>());
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

        var handler = new FulfillOrder.Handler(CreateUOW(), new FakeLogger<FulfillOrder.Handler>());
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

        var handler = new ReleaseOrder.Handler(CreateUOW(), new FakeLogger<ReleaseOrder.Handler>());
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
        var handler = new ScheduleOrder.Handler(CreateUOW(), new FakeLogger<ScheduleOrder.Handler>());
        var request = new ScheduleOrder.Command(dto.Id, scheduledDate);

        // Act
        var response = handler.Handle(request, _token).Result;
        var queried = GetOrder(dto.Id);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        queried.ScheduledDate.Should().Be(scheduledDate);

    }

    private IManufacturingUnitOfWork CreateUOW() {
        var factory = new ManufConnFactory(dbcontainer.ConnectionString);
        return new ManufacturingUnitOfWork(factory, new FakeLogger<UnitOfWork>(), new FakePublisher(), (conn, trx) => new WorkOrderRepository(new DapperConnection(conn), trx));
    }*/

    private static async Task<WorkOrderDTO> GetOrder(HttpClient client, Guid id) {
        var response = await client.GetAsync($"/orders/{id}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<WorkOrderDTO>(responseBody);
        return order!;
    }

    private static async Task<WorkOrderDTO> CreateNew(HttpClient client, NewWorkOrder expected) {
        var content = JsonContent.Create(expected);
        var createResponse = await client.PostAsync("/orders", content);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseBody = await createResponse.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<WorkOrderDTO>(responseBody);
        return order!;
    }

}
