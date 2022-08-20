using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using RoyalERP.API.Catalog.ProductClasses.Domain;
using RoyalERP.API.Tests.Unit.Common;
using System.Linq;
using Xunit;

namespace RoyalERP.API.Tests.Unit.Catalog.ProductClasses;

public class ProductClassRepositoryTests {

    private readonly IProductClassRepository _sut;
    private readonly IPublisher _publisher;
    private readonly Mock<IPublisher> _publisherMock;

    public ProductClassRepositoryTests() {

        var conn = new FakeConnection();
        var trans = new FakeTransaction();
        _sut = new ProductClassRepository(conn, trans);

        _publisherMock = new Mock<IPublisher>();
        _publisher = _publisherMock.Object;

    }

    [Fact]
    public void AddPublish_ShouldPublishCreatedEvent() {

        // Arrange
        var prodClass = CreateEntity();

        // Act
        _sut.AddAsync(prodClass).Wait();
        _sut.PublishEvents(_publisher);

        // Assert
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductClassCreated).Any());

    }

    [Fact]
    public void RemovePublish_ShouldPublishRemovedEvent() {

        // Arrange
        var prodClass = CreateEntity();
        _sut.AddAsync(prodClass).Wait();

        // Act
        _sut.RemoveAsync(prodClass).Wait();
        _sut.PublishEvents(_publisher);

        // Assert
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductClassDeleted).Any());

    }

    [Fact]
    public void UpdatePublish_ShouldPublishNameUpdatedEvent() {

        // Arrange
        var prodClass = CreateEntity();
        _sut.AddAsync(prodClass).Wait();
        prodClass.SetName("New Name");

        // Act
        _sut.UpdateAsync(prodClass).Wait();
        _sut.PublishEvents(_publisher);

        // Assert
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductClassUpdated).Any());

    }

    private static ProductClass CreateEntity() {
        return ProductClass.Create(new Faker().Random.Words());
    }

}
