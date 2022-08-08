using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using RoyalERP;
using RoyalERP.Common.Data;
using RoyalERP.Manufacturing;
using RoyalERP.Sales;
using RoyalERP.Sales.Companies;
using RoyalERP.Sales.Companies.Commands;
using RoyalERP.Sales.Companies.DTO;
using RoyalERP.Sales.Companies.Queries;
using RoyalERP_IntegrationTests.Infrastructure;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP_IntegrationTests.Sales;

public class CompanyTests : DbTests {

    [Fact]
    public async Task GetAll_ShouldReturnEmptyArray() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.GetAsync("/companies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("[]");

    }

    [Fact]
    public async Task GetAll_ShouldReturnArray() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var client = CreateClientWithAuth();

        int count = new Random().Next(5);
        for (int i = 0; i < count; i++) {
            var expected = faker.Generate();
            var json = JsonConvert.SerializeObject(expected);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var createResponse = await client.PostAsync("/companies", content);
            createResponse.EnsureSuccessStatusCode();
        }

        // Act
        var response = await client.GetAsync("/companies");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var companies = JsonConvert.DeserializeObject<CompanyDTO[]>(responseContent);
        companies.Should().NotBeNull();
        companies.Should().HaveCount(count);

    }

    // TODO: get should return not found
    // TODO: get should return created company

    [Fact]
    public async Task Create_ShouldReturnNewCompany() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();

        // Act
        var response = await client.PostAsync("/companies", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadAsStringAsync();
        var actual = JsonConvert.DeserializeObject<CompanyDTO>(result);

        actual.Should().NotBeNull();
        actual!.Name.Should().Be(expected.Name);

    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenDoesntExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.DeleteAsync($"/companies/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenSuccessful() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        createResponse.EnsureSuccessStatusCode();
        var responseStr = await createResponse.Content.ReadAsStringAsync();
        var actual = JsonConvert.DeserializeObject<CompanyDTO>(responseStr);
        actual.Should().NotBeNull();

        // Act
        var response = await client.DeleteAsync($"/companies/{actual!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

    }

}