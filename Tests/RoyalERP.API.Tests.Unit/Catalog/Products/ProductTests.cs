using FluentAssertions;
using RoyalERP.API.Catalog.Products.Domain;
using System;
using Xunit;

namespace RoyalERP.API.Tests.Unit.Catalog.Products;
public class ProductTests {

    private readonly Product _sut;

    public ProductTests() {
        _sut = new(Guid.NewGuid(), 0, "New Product", null, new());
    }

    [Fact]
    public void Create_ShouldAddEvent() {

        // Arrange
        var name = "New Product";

        // Act
        var newsut = Product.Create(name);

        // Assert
        newsut.Id.Should().NotBeEmpty();
        newsut.Events.Should().ContainSingle(e => e is Events.ProductCreated && ((Events.ProductCreated)e).Name.Equals(name));

    }

    [Fact]
    public void SetName_ShouldUpdateNameAndAddEvent() {

        // Arrange
        var newName = "New Name";

        // Act
        _sut.SetName(newName);

        // Assert
        _sut.Name.Should().Be(newName);
        _sut.Events.Should().ContainSingle(e => e is Events.ProductNameUpdated && ((Events.ProductNameUpdated)e).Name.Equals(newName));

    }

    [Fact]
    public void AddAttribute_ShouldUpdateAttributeListAndAddEvent() {

        // Arrange
        var attributeId = new Guid();

        // Act
        _sut.AddAttribute(attributeId);

        // Assert
        _sut.AttributeIds.Should().Contain(id => id == attributeId);
        _sut.Events.Should().ContainSingle(e => e is Events.ProductAttributeAdded && ((Events.ProductAttributeAdded)e).AttributeId.Equals(attributeId));

    }

    [Fact]
    public void AddAttribute_ShouldNotDoAnything_WhenAlreadyContainsAttribute() {

        // Arrange
        var attributeId = new Guid();
        var sut = new Product(Guid.NewGuid(), 0, "New Product", null, new() { attributeId });

        // Act
        sut.AddAttribute(attributeId);

        // Assert
        sut.AttributeIds.Should().ContainSingle(id => id == attributeId);
        sut.Events.Should().BeEmpty();

    }

    [Fact]
    public void RemoveAttribute_ShouldRemoveAttributeIdAndAddEvent() {

        // Arrange
        var attributeId = new Guid();
        _sut.AddAttribute(attributeId);

        // Act
        _sut.RemoveAttribute(attributeId);

        // Assert
        _sut.AttributeIds.Should().NotContain(id => id == attributeId);
        _sut.Events.Should().ContainSingle(e => e is Events.ProductAttributeRemoved && ((Events.ProductAttributeRemoved)e).AttributeId.Equals(attributeId));

    }

    [Fact]
    public void RemoveAttribute_ShouldNotDoAnything_WhenDoesNotContainAttribute() {

        // Arrange
        var attributeId = new Guid();
        var sut = new Product(Guid.NewGuid(), 0, "New Product", null, new());

        // Act
        sut.RemoveAttribute(attributeId);

        // Assert
        sut.AttributeIds.Should().BeEmpty();
        sut.Events.Should().BeEmpty();

    }

    [Fact]
    public void RemoveClass_ShouldNotDoAnything_WhenNotInClass() {

        // Arrange
        var sut = new Product(Guid.NewGuid(), 0, "New Product", null, new());

        // Act
        sut.RemoveFromProductClass();

        // Assert
        sut.ClassId.Should().BeNull();
        sut.Events.Should().BeEmpty();

    }

    [Fact]
    public void RemoveClass_ShouldSetClassIdToNull_WhenWasInClass() {

        // Arrange
        var sut = new Product(Guid.NewGuid(), 0, "New Product", Guid.NewGuid(), new());

        // Act
        sut.RemoveFromProductClass();

        // Assert
        sut.ClassId.Should().BeNull();
        sut.Events.Should().ContainSingle(e => e is Events.ProductRemovedFromClass);

    }

    [Fact]
    public void SetClass_ShouldSetNewClass_WhenNotInClass() {

        // Arrange
        Guid newClassId = Guid.NewGuid();
        var sut = new Product(Guid.NewGuid(), 0, "New Product", null, new());

        // Act
        sut.SetProductClass(newClassId);

        // Assert
        sut.ClassId.Should().NotBeNull();
        sut.Events.Should().ContainSingle(e => e is Events.ProductAddedToClass && ((Events.ProductAddedToClass)e).ProductClassId.Equals(newClassId));

    }

    [Fact]
    public void SetClass_ShouldOverwriteClass_WhenWasInClass() {

        // Arrange
        Guid newClassId = Guid.NewGuid();
        var sut = new Product(Guid.NewGuid(), 0, "New Product", Guid.NewGuid(), new());

        // Act
        sut.SetProductClass(newClassId);

        // Assert
        sut.ClassId.Should().NotBeNull();
        sut.Events.Should().ContainSingle(e => e is Events.ProductAddedToClass && ((Events.ProductAddedToClass)e).ProductClassId.Equals(newClassId));

    }

}
