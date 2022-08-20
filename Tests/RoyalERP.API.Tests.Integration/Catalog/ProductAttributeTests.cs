using FluentAssertions;
using RoyalERP.API.Tests.Integration.Infrastructure;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP.API.Tests.Integration.Catalog;

public class ProductAttributeTests : DbTests {

    [Fact]
    public void Create_ShouldReturn201() {

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
    public void Delete_ShouldReturn204_WhenEntityDoesExist() {

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
    public void GetById_ShouldReturn200_WhenEntityDoesExist() {

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
    public void SetName_ShouldReturn404_WhenEntityDoesNotExist() {

    }

    [Fact]
    public void SetName_ShouldReturn200_WhenEntityDoesExist() {

    }

}