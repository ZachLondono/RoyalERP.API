using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RoyalERP.Common.Data;
using RoyalERP.Sales.Orders.Commands;
using RoyalERP.Sales;
using RoyalERP.Sales.Companies.Domain;
using RoyalERP.Sales.Orders.Domain;
using RoyalERP.Sales.Orders.DTO;
using System;
using System.Data;
using System.Threading;
using Xunit;
using RoyalERP.Sales.Orders.Queries;

namespace RoyalERP_IntegrationTests;

public sealed partial class SalesTests : DbTests {

    private readonly CancellationToken _token;

    public SalesTests() {

        CancellationTokenSource source = new CancellationTokenSource();
        _token = source.Token;
    }

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
        var createHandler = new Create.Handler(CreateUOW());
        var createRequest = new Create.Command(new() { CustomerName = "A", Name = "B", Number = "C", VendorName = "D" });
        var createResponse = createHandler.Handle(createRequest, _token).Result;
        return (((CreatedResult)createResponse).Value as OrderDTO)!;
    }

    private OrderDTO GetOrder(Guid id) {
        var getHandler = new GetById.Handler(new SalesConnFactory(dbcontainer.ConnectionString));
        var getRequest = new GetById.Query(id);
        return getHandler.Handle(getRequest, _token).Result!;
    }

    private ISalesUnitOfWork CreateUOW() {
        var factory = new SalesConnFactory(dbcontainer.ConnectionString);
        Func<IDbConnection, IDbTransaction, IOrderRepository> orderRepoFactory = (conn, trx) => new OrderRepository(new DapperConnection(conn), trx);
        Func<IDbConnection, IDbTransaction, ICompanyRepository> companyRepoFactory = (conn, trx) => new CompanyRepository(new DapperConnection(conn), trx);
        return new SalesUnitOfWork(factory, new FakePublisher(), companyRepoFactory, orderRepoFactory);
    }

    private class SalesConnFactory : ISalesConnectionFactory {

        private readonly string _connString;
        public SalesConnFactory(string connString) {
            _connString = connString;
        }

        public IDbConnection CreateConnection() {
            return new NpgsqlConnection(_connString);
        }

    }

}