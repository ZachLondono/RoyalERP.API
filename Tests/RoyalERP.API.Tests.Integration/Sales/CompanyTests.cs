using Bogus;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using RoyalERP.API.Contracts.Companies;
using RoyalERP.API.Contracts.Orders;
using RoyalERP.API.Tests.Integration.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP.API.Tests.Integration.Sales;

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
    public async Task Update_ShouldChangeCompany() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);

        string newName = "New Name";
        string newContact = "New Contact";
        string newEmail = "New Email";

        var update = JsonContent.Create(new UpdateCompany() {
            Name = newName,
            Contact = newContact,
            Email = newEmail
        });

        // Act
        var response = await client.PutAsync($"/companies/{newOrder!.Id}", update);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Name.Should().Be(newName);

    }

    [Fact]
    public async Task UpdateAddress_ShouldChangeCompanyAddress() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);

        string newLine1 = "new Line 1";
        string newLine2 = "new Line 2";
        string newLine3 = "new Line 3";
        string newCity = "City";
        string newState = "State";
        string newZip = "Zip";

        var update = JsonContent.Create(new UpdateAddress() {
            Line1 = newLine1,
            Line2 = newLine2,
            Line3 = newLine3,
            City = newCity,
            State = newState,
            Zip = newZip,
        });

        // Act
        var response = await client.PutAsync($"/companies/{newOrder!.Id}/address", update);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Address.Line1.Should().Be(newLine1);
        updatedCompany.Address.Line2.Should().Be(newLine2);
        updatedCompany.Address.Line3.Should().Be(newLine3);
        updatedCompany.Address.City.Should().Be(newCity);
        updatedCompany.Address.State.Should().Be(newState);
        updatedCompany.Address.Zip.Should().Be(newZip);

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

    [Fact]
    public async Task SetDefault_ShouldCreateDefault() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);

        var defaultValue = new SetDefaultValue() {
            ProductId = Guid.NewGuid(),
            AttributeId = Guid.NewGuid(),
            Value = "Default Value"
        };
        var update = JsonContent.Create(defaultValue);

        // Act
        var response = await client.PutAsync($"/companies/{newOrder!.Id}/defaults", update);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Defaults.Should().ContainEquivalentOf(defaultValue);
        updatedCompany!.Defaults.Should().HaveCount(1);

    }

    [Fact]
    public async Task SetDefault_ShouldOverwriteDefault() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);

        var defaultValue = new SetDefaultValue() {
            ProductId = Guid.NewGuid(),
            AttributeId = Guid.NewGuid(),
            Value = "Default Value"
        };
        var update1 = JsonContent.Create(defaultValue);
        await client.PutAsync($"/companies/{newOrder!.Id}/defaults", update1);
        
        defaultValue = new SetDefaultValue() {
            ProductId = Guid.NewGuid(),
            AttributeId = Guid.NewGuid(),
            Value = "New Default Value"
        };
        var update2 = JsonContent.Create(defaultValue);

        // Act
        var response = await client.PutAsync($"/companies/{newOrder!.Id}/defaults", update2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Defaults.Should().ContainEquivalentOf(defaultValue);
        updatedCompany!.Defaults.Should().HaveCount(1);

    }

    [Fact]
    public async Task SetDefault_ShouldReturn404WhenEntityNotExist() {

        // Arrange
        var client = CreateClientWithAuth();
        var defaultValue = new SetDefaultValue() {
            ProductId = Guid.NewGuid(),
            AttributeId = Guid.NewGuid(),
            Value = "Default Value"
        };
        var update = JsonContent.Create(defaultValue);

        // Act
        var response = await client.PutAsync($"/companies/{Guid.NewGuid()}/defaults", update);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task RemoveDefault_ShouldRemoveDefault() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);

        var defaultValue = new SetDefaultValue() {
            ProductId = Guid.NewGuid(),
            AttributeId = Guid.NewGuid(),
            Value = "Default Value"
        };
        var update1 = JsonContent.Create(defaultValue);
        await client.PutAsync($"/companies/{newOrder!.Id}/defaults", update1);


        // Act
        var response = await client.DeleteAsync($"/companies/{newOrder!.Id}/defaults/{defaultValue.ProductId}/{defaultValue.AttributeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Defaults.Should().HaveCount(0);

    }

    [Fact]
    public async Task RemoveDefault_ShouldReturn404WhenEntityNotExist() {

        // Arrange
        var client = CreateClientWithAuth();

        // Act
        var response = await client.DeleteAsync($"/companies/{Guid.NewGuid()}/defaults/{Guid.NewGuid()}/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }

    [Fact]
    public async Task SetInfo_ShouldSetInfo() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);

        string field = "Key1";
        string value = "Value1";

        // Act
        var response = await client.PutAsync($"/companies/{newOrder!.Id}/info/{field}/{value}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Info.Should().Contain(new KeyValuePair<string, string>(field, value));

    }

    [Fact]
    public async Task RemoveInfo_ShouldRemoveInfo() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);

        string field = "Key1";
        string value = "Value1";
        await client.PutAsync($"/companies/{newOrder!.Id}/info/{field}/{value}", null);

        // Act
        var response = await client.DeleteAsync($"/companies/{newOrder!.Id}/info/{field}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Info.Should().BeEmpty();

    }

    [Fact]
    public async Task AddContact_ShouldAddContact() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);

        var newContact = new NewContact() {
            Name = "Name",
            Email = "Email",
            Phone = "Phone",
            Roles = new[] { "Roles" }
        };

        var update = JsonContent.Create(newContact);

        // Act
        var response = await client.PostAsync($"/companies/{newOrder!.Id}/contacts/", update);


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCompany = await response.Content.ReadFromJsonAsync<CompanyDTO>();
        updatedCompany!.Contacts.Should().ContainSingle(c => c.Name == newContact.Name && c.Email == newContact.Email && c.Phone == newContact.Phone && c.Roles.Contains(newContact.Roles[0]));

    }

    [Fact]
    public async Task RemoveContact_ShouldRemoveContact() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);
        var newContact = new NewContact() {
            Name = "Name",
            Email = "Email",
            Phone = "Phone",
            Roles = new[] { "Roles" }
        };
        var update = JsonContent.Create(newContact);
        var createContactResponse = await client.PostAsync($"/companies/{newOrder!.Id}/contacts/", update);
        var updatedCompany = await createContactResponse.Content.ReadFromJsonAsync<CompanyDTO>();
        var contact = updatedCompany!.Contacts.First();

        // Act
        var response = await client.DeleteAsync($"/companies/{newOrder!.Id}/contacts/{contact.Id}");


        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedCompany!.Contacts.Should().HaveCount(0);

    }

    [Fact]
    public async Task AddRoleToContact_ShouldAddRole() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);
        var newContact = new NewContact() {
            Name = "Name",
            Email = "Email",
            Phone = "Phone",
            Roles = new[] { "Roles" }
        };
        var update = JsonContent.Create(newContact);
        var createContactResponse = await client.PostAsync($"/companies/{newOrder!.Id}/contacts/", update);
        var updatedCompany = await createContactResponse.Content.ReadFromJsonAsync<CompanyDTO>();
        var contact = updatedCompany!.Contacts.First();

        var role = new ContactRole() {
            Role = "owner"
        };
        var roleAdd = JsonContent.Create(role);

        // Act
        var response = await client.PostAsync($"/companies/{newOrder!.Id}/contacts/{contact.Id}/roles", roleAdd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedCompany!.Contacts.Should().HaveCount(2);
        var updatedContact = updatedCompany!.Contacts.First();
        updatedContact.Roles.Should().ContainSingle(r => r.Equals(role.Role));

    }

    [Fact]
    public async Task RemoveRoleToContact_ShouldRemoveExistingRole() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var expected = faker.Generate();
        var json = JsonConvert.SerializeObject(expected);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var client = CreateClientWithAuth();
        var createResponse = await client.PostAsync("/companies", content);
        var createdResult = await createResponse.Content.ReadAsStringAsync();
        var newOrder = JsonConvert.DeserializeObject<CompanyDTO>(createdResult);
        var newContact = new NewContact() {
            Name = "Name",
            Email = "Email",
            Phone = "Phone",
            Roles = new[] { "existing" }
        };
        var update = JsonContent.Create(newContact);
        var createContactResponse = await client.PostAsync($"/companies/{newOrder!.Id}/contacts/", update);
        var updatedCompany = await createContactResponse.Content.ReadFromJsonAsync<CompanyDTO>();
        var contact = updatedCompany!.Contacts.First();

        // Act
        var response = await client.DeleteAsync($"/companies/{newOrder!.Id}/contacts/{contact.Id}/roles/existing");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedCompany!.Contacts.Should().HaveCount(0);

    }

}