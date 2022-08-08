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
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });


        // Act
        var response = await client.DeleteAsync($"/workorders/{dto.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        // TODO: check that it is nolonger accessible

    }

    [Fact]
    public async Task Cancel_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });


        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/cancel", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // TODO: check that get returns updated status
    }

    [Fact]
    public async Task Fulfill_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });


        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/fulfill", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // TODO: check that get returns updated status

    }

    [Fact]
    public async Task Release_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });


        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/release", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // TODO: check that get returns updated status

    }

    /*[Fact]
    public async Task Schedule_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var client = CreateClientWithAuth();
        var dto = await CreateNew(client, new() {
            Number = "OT123",
            Name = "ABC's Order",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        });

        // Act
        var response = await client.PutAsync($"/workorders/{dto.Id}/schedule", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // TODO: check that get returns updated status

    }*/

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
