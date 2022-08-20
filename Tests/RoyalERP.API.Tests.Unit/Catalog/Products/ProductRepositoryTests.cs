using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using RoyalERP.API.Catalog.Products.Domain;
using RoyalERP.API.Tests.Unit.Common;
using System;
using System.Data;
using System.Linq;
using Xunit;

namespace RoyalERP.API.Tests.Unit.Catalog.Products;

public class ProductRepositoryTests {

    private IProductRepository _sut;
    private IPublisher _publisher;
    private Mock<IPublisher> _publisherMock;

    public ProductRepositoryTests() {

        var conn = new FakeConnection();
        var trans = new FakeTransaction();
        _sut = new ProductRepository(conn, trans);

        _publisherMock = new Mock<IPublisher>();
        _publisher = _publisherMock.Object;

    }

    [Fact]
    public void AddPublish_ShouldPublishCreatedEvent() {

        // Arrange
        var prod = CreateEntity();

        // Act
        _sut.AddAsync(prod).Wait();
        _sut.PublishEvents(_publisher);

        // Assert
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductCreated).Any() );

    }

    [Fact]
    public void RemovePublish_ShouldPublishRemovedEvent() {

        // Arrange
        var prod = CreateEntity();
        _sut.AddAsync(prod).Wait();

        // Act
        _sut.RemoveAsync(prod).Wait();
        _sut.PublishEvents(_publisher);

        // Assert
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductDeleted).Any());

    }

    [Fact]
    public void UpdatePublish_ShouldPublishNameUpdatedEvent() {

        // Arrange
        var prod = CreateEntity();
        _sut.AddAsync(prod).Wait();
        prod.SetName("New Name");

        // Act
        _sut.UpdateAsync(prod).Wait();
        _sut.PublishEvents(_publisher);

        // Assert
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductNameUpdated).Any());

    }

    [Fact]
    public void UpdatePublish_ShouldPublishAttributeAddedEvent() {

        // Arrange
        var prod = CreateEntity();
        _sut.AddAsync(prod).Wait();
        prod.AddAttribute(Guid.NewGuid());

        // Act
        _sut.UpdateAsync(prod).Wait();
        _sut.PublishEvents(_publisher);

        // Assert
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductAttributeAdded).Any());

    }

    [Fact]
    public void UpdatePublish_ShouldPublishAttributeRemovedEvent() {

        // Arrange
        var prod = CreateEntity();
        _sut.AddAsync(prod).Wait();
        var id = Guid.NewGuid();
        prod.AddAttribute(id);
        var result = prod.RemoveAttribute(id);

        // Act
        _sut.UpdateAsync(prod).Wait();
        _sut.PublishEvents(_publisher);

        // Assert
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductAttributeRemoved).Any());

    }

    private static Product CreateEntity() {
        return Product.Create(new Faker().Random.Words());
    }

}