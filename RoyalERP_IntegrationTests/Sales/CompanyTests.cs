using Bogus;
using FluentAssertions;
using Newtonsoft.Json;
using RoyalERP.Sales.Companies.DTO;
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

    [Fact]
    public async Task Get_ShouldReturnNotFound() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.GetAsync($"/companies/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task Get_ShouldReturnExistingCompany() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var company = faker.Generate();
        var json = JsonConvert.SerializeObject(company);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var expectedStr = await createResponse.Content.ReadAsStringAsync();
        var expected = JsonConvert.DeserializeObject<CompanyDTO>(expectedStr);

        // Act
        var response = await client.GetAsync($"/companies/{expected!.Id}");

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        var actual = JsonConvert.DeserializeObject<CompanyDTO>(responseContent);

        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected);

    }

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

        var check = await client.GetAsync($"/companies/{actual.Id}");
        check.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

}