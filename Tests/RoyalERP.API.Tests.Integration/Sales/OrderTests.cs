using FluentAssertions;
using RoyalERP.API.Sales.Orders.Domain;
using RoyalERP.API.Contracts.Orders;
using System;
using Xunit;
using RoyalERP.API.Tests.Integration.Infrastructure;
using System.Net.Http.Json;
using System.Net;
using System.Threading.Tasks;
using RoyalERP.API.Contracts.Companies;
using System.Net.Http;
using Newtonsoft.Json;
using Bogus;
using System.Collections.Generic;
using System.Linq;

namespace RoyalERP.API.Tests.Integration.Sales;

public class OrderTests : DbTests {


    private readonly Faker _faker = new();

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
    public async Task DeleteItem_ShouldReturnNotFound_WhenOrderDoesntExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.DeleteAsync($"/orders/{Guid.NewGuid()}/items/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task DeleteItem_ShouldReturnNotFound_WhenItemDoesntExist() {

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
        var response = await client.DeleteAsync($"/orders/{dto.Id}/items/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

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
        updated.Status.Should().Be(OrderStatus.Cancelled.ToString());

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
        updated.Status.Should().Be(OrderStatus.Completed.ToString());

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
        updated.Status.Should().Be(OrderStatus.Confirmed.ToString());

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

    [Fact]
    public async Task AddItem_ShouldAddItemToOrder() {

        // Arrange
        var client = CreateClientWithAuth();
        var expected = RandomNewOrder();
        var dto = await CreateNew(client, expected);

        var newItem = RandomNewItem();
        var content = JsonContent.Create(newItem);

        // Act
        var response = await client.PostAsync($"/orders/{dto.Id}/items", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contentStr = await response.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<OrderDetails>(contentStr);
        order.Should().NotBeNull();
        order!.Items.Should().Contain(i => i.ProductName.Equals(newItem.ProductName) && i.Quantity.Equals(newItem.Quantity));
        //order.Items.First().Properties.Should().BeEquivalentTo(newItem.Properties);

        var getorder = await GetOrder(client, dto.Id);
        getorder.Should().NotBeNull();
        getorder!.Items.Should().Contain(i => i.ProductName.Equals(newItem.ProductName) && i.Quantity.Equals(newItem.Quantity));
        //getorder.Items.First().Properties.Should().BeEquivalentTo(newItem.Properties);

    }

    [Fact]
    public async Task AddItem_ShouldAddItemToOrder_WhenAddingMultiple() {

        // Arrange
        var client = CreateClientWithAuth();
        var expected = RandomNewOrder();
        var dto = await CreateNew(client, expected);

        int count = 5;

        List<NewItem> items = new();

        // Act
        for (int i = 0; i < count; i++) {
            var newItem = RandomNewItem();
            items.Add(newItem);
            var content = JsonContent.Create(newItem);
            var response = await client.PostAsync($"/orders/{dto.Id}/items", content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Assert
        var order = await GetOrder(client, dto.Id);
        order.Items.Should().HaveCount(count);
        foreach (var item in items) {
            order.Items.Should().Contain(i => i.ProductName.Equals(item.ProductName) && i.Quantity.Equals(item.Quantity));
        }

    }

    private NewItem RandomNewItem() => new() {
        ProductName = _faker.Commerce.ProductName(),
        Quantity = _faker.Random.Number(min: 0),
        Properties = new() { { _faker.Random.String(), _faker.Random.String() } }
    };

    private NewOrder RandomNewOrder() => new() {
        Name = _faker.Random.Word(),
        Number = _faker.Random.Word(),
        CustomerName = _faker.Company.CompanyName(),
        VendorName = _faker.Company.CompanyName()
    };

    private static async Task<OrderDetails> GetOrder(HttpClient client, Guid id) {
        var response = await client.GetAsync($"/orders/{id}");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<OrderDetails>(responseBody);
        return order!;
    }

    private static async Task<OrderDetails> CreateNew(HttpClient client, NewOrder expected) {
        var content = JsonContent.Create(expected);
        var createResponse = await client.PostAsync("/orders", content);
        createResponse.EnsureSuccessStatusCode();
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseBody = await createResponse.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<OrderDetails>(responseBody);
        return order!;
    }

}