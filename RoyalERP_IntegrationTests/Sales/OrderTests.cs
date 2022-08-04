using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RoyalERP.Sales.Orders.Commands;
using RoyalERP.Sales.Orders.Domain;
using RoyalERP.Sales.Orders.DTO;
using System;
using Xunit;
using RoyalERP.Sales.Orders.Queries;
using Bogus;

namespace RoyalERP_IntegrationTests.Sales;

public sealed partial class OrderTests : SalesTests {

    [Fact]
    public void Create_ShouldReturnNewWorkOrder() {

        // Arrange
        var expected = new NewOrder() {
            Name = "Order Name",
            Number = "OT123",
            CustomerName = "Customer Name",
            VendorName = "Vendor Name"
        };

        var handler = new Create.Handler(CreateUOW());
        var request = new Create.Command(expected);

        // Act
        var response = handler.Handle(request, _token).Result;

        // Assert
        response.Should().BeOfType<CreatedResult>();

        var created = response as CreatedResult;

        created!.Value.Should().NotBeNull();
        created!.Value.Should().BeOfType<OrderDTO>();

        var returnedEntity = created.Value as OrderDTO;
        returnedEntity!.Should().NotBeNull();

        var actual = GetOrder(returnedEntity!.Id);
        actual.Name.Should().Be(expected.Name);
        actual.Number.Should().Be(expected.Number);
        actual.Status.Should().Be(OrderStatus.Unconfirmed);

        returnedEntity.Should().BeEquivalentTo(actual);

    }

    [Fact]
    public void Create_ShouldCreateNewCompany_WhenNameDoesNotExist() {

        // Arrange
        var fake = new Faker<NewOrder>()
                .RuleFor(o => o.CustomerId, f => null)
                .RuleFor(o => o.CustomerId, f => null)
                .RuleFor(o => o.CustomerName, f => f.Company.CompanyName())
                .RuleFor(o => o.VendorName, f => f.Company.CompanyName())
                .RuleFor(o => o.Number, f => f.Random.Number(999999).ToString())
                .RuleFor(o => o.Name, f => f.Name.FirstName());

        var expected = fake.Generate();

        var handler = new Create.Handler(CreateUOW());
        var request = new Create.Command(expected);

        // Act
        var response = handler.Handle(request, _token).Result;

        // Assert
        response.Should().BeOfType<CreatedResult>();
        var okresult = response as CreatedResult;
        var order = okresult!.Value as OrderDTO;

        var customerQuery = new RoyalERP.Sales.Companies.Queries.GetById.Query(order!.CustomerId);
        var vendorQuery = new RoyalERP.Sales.Companies.Queries.GetById.Query(order.VendorId);
        var queryHandler = new RoyalERP.Sales.Companies.Queries.GetById.Handler(new SalesConnFactory(dbcontainer.ConnectionString));

        var customer = queryHandler.Handle(customerQuery, _token).Result;
        var vendor = queryHandler.Handle(vendorQuery, _token).Result;

        customer.Should().NotBeNull();
        customer!.Name.Should().Be(expected.CustomerName);
        vendor.Should().NotBeNull();
        vendor!.Name.Should().Be(expected.VendorName);

    }

    [Fact]
    public void Delete_ShouldReturnNotFound_WhenDoesntExist() {

        // Arrange
        var orderId = Guid.NewGuid();
        var handler = new Delete.Handler(CreateUOW());
        var request = new Delete.Command(orderId);

        // Act
        var response = handler.Handle(request, _token).Result;

        // Assert
        response.Should().BeOfType<NotFoundResult>();

    }

    [Fact]
    public void Delete_ShouldReturnNoContent_WhenSuccessful() {

        // Arrange
        var dto = CreateNew();

        var handler = new Delete.Handler(CreateUOW());
        var request = new Delete.Command(dto.Id);

        var getHandler = new GetById.Handler(new SalesConnFactory(dbcontainer.ConnectionString));
        var getRequest = new GetById.Query(dto.Id);

        // Act
        var response = handler.Handle(request, _token).Result;
        var getResponse = getHandler.Handle(getRequest, _token).Result;

        // Assert
        response.Should().BeOfType<NoContentResult>();
        getResponse.Should().BeNull();

    }

    [Fact]
    public void Cancel_ShouldUpdateReturnOk_AndUpdateDb() {

        // Arrange
        var dto = CreateNew();

        var handler = new CancelOrder.Handler(CreateUOW());
        var request = new CancelOrder.Command(dto.Id);

        // Act
        var response = handler.Handle(request, _token).Result;
        var queried = GetOrder(dto.Id);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        queried.Status.Should().Be(OrderStatus.Cancelled);

    }

    [Fact]
    public void Complete_ShouldUpdateReturnOk() {

        // Arrange
        var dto = CreateNew();

        var handler = new CompleteOrder.Handler(CreateUOW());
        var request = new CompleteOrder.Command(dto.Id);

        // Act
        var response = handler.Handle(request, _token).Result;
        var queried = GetOrder(dto.Id);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        queried.CompletedDate.Should().Be(DateTime.Today);
        queried.Status.Should().Be(OrderStatus.Completed);

    }

    [Fact]
    public void Confirm_ShouldUpdateReturnOk() {

        // Arrange
        var dto = CreateNew();

        var handler = new ConfirmOrder.Handler(CreateUOW());
        var request = new ConfirmOrder.Command(dto.Id);

        // Act
        var response = handler.Handle(request, _token).Result;
        var queried = GetOrder(dto.Id);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        queried.ConfirmedDate.Should().Be(DateTime.Today);
        queried.Status.Should().Be(OrderStatus.Confirmed);

    }

    private OrderDTO CreateNew() {

        var fake = new Faker<NewOrder>()
                .RuleFor(o => o.CustomerId, f => null)
                .RuleFor(o => o.CustomerId, f => null)
                .RuleFor(o => o.CustomerName, f => f.Company.CompanyName())
                .RuleFor(o => o.VendorName, f => f.Company.CompanyName())
                .RuleFor(o => o.Number, f => f.Random.Number(999999).ToString())
                .RuleFor(o => o.Name, f => f.Name.FirstName());

        var newOrder = fake.Generate();

        var createHandler = new Create.Handler(CreateUOW());
        var createRequest = new Create.Command(newOrder);
        var createResponse = createHandler.Handle(createRequest, _token).Result;
        return (((CreatedResult)createResponse).Value as OrderDTO)!;
    }

    private OrderDTO GetOrder(Guid id) {
        var getHandler = new GetById.Handler(new SalesConnFactory(dbcontainer.ConnectionString));
        var getRequest = new GetById.Query(id);
        return getHandler.Handle(getRequest, _token).Result!;
    }

}