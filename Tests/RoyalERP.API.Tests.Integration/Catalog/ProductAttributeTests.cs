using Bogus;
using FluentAssertions;
using Newtonsoft.Json;
using RoyalERP.API.Contracts.ProductAttributes;
using RoyalERP.API.Tests.Integration.Infrastructure;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP.API.Tests.Integration.Catalog;

public class ProductAttributeTests : DbTests {

    [Fact]
    public async Task Create_ShouldReturn201() {

        // Arrange
        var faker = new Faker<NewProductAttribute>().RuleFor(c => c.Name, f => f.Commerce.ProductName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();

        // Act
        var response = await client.PostAsync("/attributes", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadAsStringAsync();
        var actual = JsonConvert.DeserializeObject<ProductAttributeDTO>(result);

        actual.Should().NotBeNull();
        actual!.Name.Should().Be(expected.Name);

    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenEntityDoesNotExist() {
        
        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.DeleteAsync($"/attributes/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Delete_ShouldReturn204_WhenEntityDoesExist() {
        
        // Arrange
        var client = CreateClientWithAuth();
        var created = await CreateEntity(client);

        // Act
        var response = await client.DeleteAsync($"/attributes/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenEntityDoesNotExist() {
        
        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.GetAsync($"/attributes/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task GetById_ShouldReturn200_WhenEntityDoesExist() {

        // Arrange
        var client = CreateClientWithAuth();
        var created = await CreateEntity(client);

        // Act
        var response = await client.GetAsync($"/attributes/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }

    [Fact]
    public async Task GetAll_ShouldReturn200Async() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.GetAsync("/attributes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("[]");

    }

    [Fact]
    public async Task SetName_ShouldReturn404_WhenEntityDoesNotExist() {

        // Arrange
        var client = CreateClientWithAuth();
        var newName = new Faker().Commerce.ProductName();
        var nameUpdate = new ProductAttributeUpdate() {
            Name = newName
        };
        var json = JsonConvert.SerializeObject(nameUpdate);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync($"/attributes/{Guid.NewGuid()}/", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task SetName_ShouldReturn200_WhenEntityDoesExist() {

        // Arrange
        var client = CreateClientWithAuth();
        var created = await CreateEntity(client);
        var newName = new Faker().Commerce.ProductName();
        var nameUpdate = new ProductAttributeUpdate() {
            Name = newName
        };
        var json = JsonConvert.SerializeObject(nameUpdate);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PutAsync($"/attributes/{created.Id}/", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }

    private static async Task<ProductAttributeDTO> CreateEntity(HttpClient client) {

        // Create request body
        var faker = new Faker<NewProductAttribute>().RuleFor(c => c.Name, f => f.Commerce.ProductName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Send request
        var createResponse = await client.PostAsync("/attributes", content);
        createResponse.EnsureSuccessStatusCode();

        // Get result from response
        var result = await createResponse.Content.ReadAsStringAsync();
        var created = JsonConvert.DeserializeObject<ProductAttributeDTO>(result);
        if (created is null) throw new InvalidDataContractException();
        return created;

    }
}