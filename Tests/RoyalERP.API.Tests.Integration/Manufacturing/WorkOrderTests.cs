using FluentAssertions;
using Newtonsoft.Json;
using RoyalERP.API.Manufacturing.WorkOrders.Domain;
using RoyalERP.Contracts.WorkOrders;
using RoyalERP_IntegrationTests.Infrastructure;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP_IntegrationTests.Manufacturing;

public class WorkOrderTests : DbTests {

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenDoesntExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var result = await client.DeleteAsync($"/workorders/{Guid.NewGuid()}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenSuccessful() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            SalesOrderId = Guid.NewGuid(),
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });


        // Act
        var response = await client.DeleteAsync($"/workorders/{dto.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var check = await client.GetAsync($"/workorders/{dto.Id}");
        check.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Cancel_ShouldReturnNotFound_WhenIdDoesNotExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.PutAsync($"/workorders/{Guid.NewGuid()}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Cancel_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            SalesOrderId = Guid.NewGuid(),
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });


        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await GetOrder(client, dto.Id);
        order.Status.Should().Be(WorkOrderStatus.Cancelled.ToString());
    }

    [Fact]
    public async Task Fulfill_ShouldReturnNotFound_WhenIdDoesNotExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.PutAsync($"/workorders/{Guid.NewGuid()}/fulfill", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Fulfill_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            SalesOrderId = Guid.NewGuid(),
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });


        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/fulfill", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await GetOrder(client, dto.Id);
        order.Status.Should().Be(WorkOrderStatus.Fulfilled.ToString());

    }

    [Fact]
    public async Task Release_ShouldReturnNotFound_WhenIdDoesNotExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.PutAsync($"/workorders/{Guid.NewGuid()}/release", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Release_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            SalesOrderId = Guid.NewGuid(),
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });


        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/release", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await GetOrder(client, dto.Id);
        order.Status.Should().Be(WorkOrderStatus.InProgress.ToString());

    }

    [Fact]
    public async Task Schedule_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            SalesOrderId = Guid.NewGuid(),
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });
        var scheduledDate = DateTime.Today;
        var content = JsonContent.Create(new WorkOrderSchedule() {
            ScheduledDate = scheduledDate
        });

        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/schedule", content) ;

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await GetOrder(client, dto.Id);
        order.ScheduledDate.Should().Be(scheduledDate);

    }



    [Fact]
    public async Task SetNote_ShouldUpdateNote() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            SalesOrderId = Guid.NewGuid(),
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });
        string note = "Order Note";
        var content = JsonContent.Create(new WorkOrderNote() {
            Note = note
        });

        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/note", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await GetOrder(client, dto.Id);
        order.Note.Should().Be(note);

    }

    private static async Task<WorkOrderDTO> GetOrder(HttpClient client, Guid id) {
        var response = await client.GetAsync($"/workorders/{id}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<WorkOrderDTO>(responseBody);
        return order!;
    }

    private static async Task<WorkOrderDTO> CreateNew(HttpClient client, NewWorkOrder expected) {
        var content = JsonContent.Create(expected);
        var createResponse = await client.PostAsync("/workorders", content);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseBody = await createResponse.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<WorkOrderDTO>(responseBody);
        return order!;
    }

}
