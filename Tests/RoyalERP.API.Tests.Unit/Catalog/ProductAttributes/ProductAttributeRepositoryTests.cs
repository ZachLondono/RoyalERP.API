using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using RoyalERP.API.Catalog.ProductAttributes.Domain;
using RoyalERP.API.Tests.Unit.Common;
using System.Linq;
using Xunit;

namespace RoyalERP.API.Tests.Unit.Catalog.ProductAttributes;

public class ProductAttributeRepositoryTests {

    private readonly IProductAttributeRepository _sut;
    private readonly IPublisher _publisher;
    private readonly Mock<IPublisher> _publisherMock;

    public ProductAttributeRepositoryTests() {

        var conn = new FakeConnection();
        var trans = new FakeTransaction();
        _sut = new ProductAttributeRepository(conn, trans);

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
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductAttributeCreated).Any());

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
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductAttributeDeleted).Any());

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
        _publisherMock.Invocations.Should().Contain(i => i.Arguments.Select(o => o is Events.ProductAttributeUpdated).Any());

    }

    private static ProductAttribute CreateEntity() {
        return ProductAttribute.Create(new Faker().Random.Words());
    }

}