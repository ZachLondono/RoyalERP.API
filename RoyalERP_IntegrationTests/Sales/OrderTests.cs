using FluentAssertions;
using RoyalERP.Sales.Orders.Domain;
using RoyalERP.Sales.Orders.DTO;
using System;
using Xunit;
using RoyalERP_IntegrationTests.Infrastructure;
using System.Net.Http.Json;
using System.Net;
using System.Threading.Tasks;
using RoyalERP.Sales.Companies.DTO;
using System.Net.Http;
using Newtonsoft.Json;

namespace RoyalERP_IntegrationTests.Sales;

public class OrderTests : DbTests {

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenIdDoesNotExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.GetAsync($"/orders/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Create_ShouldReturnNewOrder() {

        // Arrange
        var client = CreateClientWithAuth();
        var expected = new NewOrder() {
            Name = "Order Name",
            Number = "OT123",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        };

        var content = JsonContent.Create(expected);

        // Act
        var response = await client.PostAsync("/orders", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseContent = await response.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<OrderDetails>(responseContent);
        order.Should().NotBeNull();

        order!.Name.Should().BeEquivalentTo(expected.Name);
        order.Number.Should().BeEquivalentTo(expected.Number);
        order.CustomerId.Should().NotBeEmpty();
        order.VendorId.Should().NotBeEmpty();

    }

    [Fact]
    public async Task Create_ShouldCreateNewCompany_WhenNameDoesNotExist() {

        // Arrange
        var client = CreateClientWithAuth();
        var expected = new NewOrder() {
             Name = "Order Name",
             Number = "OT123",
             CustomerName = "Customer Name",
             VendorName = "Vendor Name"
        };
        var dto = await  CreateNew(client, expected);
        var customerId = dto!.CustomerId;
        var vendorId = dto.VendorId;

        // Act
        var customerResponse = await client.GetAsync($"/companies/{customerId}");
        var vendorResponse = await client.GetAsync($"/companies/{vendorId}");

        // Assert
        customerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var customerContent = await customerResponse.Content.ReadFromJsonAsync<CompanyDTO>();
        customerContent.Should().NotBeNull();
        customerContent!.Name.Should().Be(expected.CustomerName);

        vendorResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var vendorContent = await vendorResponse.Content.ReadFromJsonAsync<CompanyDTO>();
        vendorContent.Should().NotBeNull();
        vendorContent!.Name.Should().Be(expected.VendorName);

    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenDoesntExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.DeleteAsync($"/orders/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleted() {

        // Arrange
        var client = CreateClientWithAuth();
        var neworder = new NewOrder() {
            Name = "Order Name",
            Number = "OT123",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        };
        var dto = await CreateNew(client, neworder);

        // Act
        var response = await client.DeleteAsync($"/orders/{dto.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var check = await client.GetAsync($"/orders/{dto.Id}");
        check.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Cancel_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var expected = new NewOrder() {
            Name = "Order Name",
            Number = "OT123",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        };
        var dto = await CreateNew(client, expected);

        // Act
        var response = await client.PutAsync($"/orders/{dto.Id}/cancel", null);
        var updated = await GetOrder(client, dto.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updated.Status.Should().Be(OrderStatus.Cancelled);
        // TODO: check that it is nolonger accessible

    }

    [Fact]
    public async Task Complete_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var expected = new NewOrder() {
            Name = "Order Name",
            Number = "OT123",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        };
        var dto = await CreateNew(client, expected);

        // Act
        var response = await client.PutAsync($"/orders/{dto.Id}/complete", null);
        var updated = await GetOrder(client, dto.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updated.Status.Should().Be(OrderStatus.Completed);

    }

    [Fact]
    public async Task Complete_ShouldReturnNotFound_WhenIdDoesNotExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.PutAsync($"/orders/{Guid.NewGuid()}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Confirm_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var expected = new NewOrder() {
            Name = "Order Name",
            Number = "OT123",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        };
        var dto = await CreateNew(client, expected);

        // Act
        var response = await client.PutAsync($"/orders/{dto.Id}/confirm", null);
        var updated = await GetOrder(client, dto.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updated.Status.Should().Be(OrderStatus.Confirmed);

    }

    [Fact]
    public async Task Confirm_ShouldReturnNotFound_WhenIdDoesNotExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.PutAsync($"/orders/{Guid.NewGuid()}/confirm", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    private static async Task<OrderDetails> GetOrder(HttpClient client, Guid id) {
        var response = await client.GetAsync($"/orders/{id}");
        var responseBody = await response.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<OrderDetails>(responseBody);
        return order!;
    }

    private static async Task<OrderDetails> CreateNew(HttpClient client, NewOrder expected) {
        var content = JsonContent.Create(expected);
        var createResponse = await client.PostAsync("/orders", content);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseBody = await createResponse.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<OrderDetails>(responseBody);
        return order!;
    }

}