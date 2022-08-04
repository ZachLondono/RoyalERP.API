using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Companies.Commands;
using RoyalERP.Sales.Companies.DTO;
using RoyalERP.Sales.Companies.Queries;
using RoyalERP_IntegrationTests.Infrastructure;
using System;
using Xunit;

namespace RoyalERP_IntegrationTests.Sales;

public class CompanyTests : SalesTests {

    [Fact]
    public void Create_ShouldReturnNewWorkOrder() {

        // Arrange
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var newCompany = faker.Generate();

        var handler = new Create.Handler(CreateUOW(), new FakeLogger<Create.Handler>());
        var request = new Create.Command(newCompany);

        var getHandler = new GetById.Handler(new SalesConnFactory(dbcontainer.ConnectionString));

        // Act
        var response = handler.Handle(request, _token).Result;

        // Assert
        response.Should().BeOfType<CreatedResult>();

        var created = response as CreatedResult;

        created!.Value.Should().NotBeNull();
        created!.Value.Should().BeOfType<CompanyDTO>();

        var returnedEntity = created.Value as CompanyDTO;
        returnedEntity!.Should().NotBeNull();

        var actual = getHandler.Handle(new GetById.Query(returnedEntity!.Id), _token).Result;

        returnedEntity.Should().BeEquivalentTo(actual);

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
        var faker = new Faker<NewCompany>().RuleFor(c => c.Name, f => f.Company.CompanyName());
        var newCompany = faker.Generate();

        var createHandler = new Create.Handler(CreateUOW(), new FakeLogger<Create.Handler>());
        var createRequest = new Create.Command(newCompany);

        var result = (createHandler.Handle(createRequest, _token).Result as CreatedResult)!.Value as CompanyDTO;

        var deleteHandler = new Delete.Handler(CreateUOW(), new FakeLogger<Delete.Handler>());
        var deleteRequest = new Delete.Command(result!.Id);

        var getHandler = new GetById.Handler(new SalesConnFactory(dbcontainer.ConnectionString));
        var getRequest = new GetById.Query(result.Id);

        // Act
        var response = deleteHandler.Handle(deleteRequest, _token).Result;
        var getResponse = getHandler.Handle(getRequest, _token).Result;

        // Assert
        response.Should().BeOfType<NoContentResult>();
        getResponse.Should().BeNull();

    }

}