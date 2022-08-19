using FluentAssertions;
using RoyalERP.API.Catalog.Products.Domain;
using System;

namespace RoyalERP.API.Tests.Unit.Catalog.Products;
public class ProductTests {

    private readonly Product _sut;

    public ProductTests() {
        _sut = new(Guid.NewGuid(), 0, "New Product", new());
    }

    public void Create_ShouldAddEvent() {

        // Arrange
        var name = "New Product";

        // Act
        var newsut = Product.Create(name);

        // Assert
        newsut.Id.Should().NotBeEmpty();
        _sut.Events.Should().ContainSingle(e => e is Events.ProductCreated && ((Events.ProductCreated)e).Name.Equals(name));

    }

    public void SetName_ShouldUpdateNameAndAddEvent() {

        // Arrange
        var newName = "New Name";

        // Act
        _sut.SetName(newName);

        // Assert
        _sut.Name.Should().Be(newName);
        _sut.Events.Should().ContainSingle(e => e is Events.ProductNameUpdated && ((Events.ProductNameUpdated)e).Name.Equals(newName));

    }

    public void AddAttribute_ShouldUpdateAttributeListAndAddEvent() {

        // Arrange
        var attributeId = new Guid();

        // Act
        _sut.AddAttribute(attributeId);

        // Assert
        _sut.AttributeIds.Should().Contain(id => id == attributeId);
        _sut.Events.Should().ContainSingle(e => e is Events.ProductAttributeAdded && ((Events.ProductAttributeAdded)e).AttributeId.Equals(attributeId));

    }

    public void RemoveAttribute() {

        // Arrange
        var attributeId = new Guid();
        _sut.AddAttribute(attributeId);

        // Act
        _sut.RemoveAttribute(attributeId);

        // Assert
        _sut.AttributeIds.Should().NotContain(id => id == attributeId);
        _sut.Events.Should().ContainSingle(e => e is Events.ProductAttributeRemoved && ((Events.ProductAttributeRemoved)e).AttributeId.Equals(attributeId));

    }

}
