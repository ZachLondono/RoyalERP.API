using FluentAssertions;
using RoyalERP.API.Catalog.ProductAttributes.Domain;
using Xunit;

namespace RoyalERP.API.Tests.Unit.Catalog.ProductAttributes;

public class ProductAttributeTests {

    [Fact]
    public void Create_ShouldSetNameAndCreateEvent() {

        // Arrange
        var name = "Product Attribute Name";

        // Act
        var sut = ProductAttribute.Create(name);

        // Assert
        sut.Name.Should().Be(name);
        sut.Events.Should().ContainSingle(e => e is Events.ProductAttributeCreated && ((Events.ProductAttributeCreated)e).Name.Equals(name));

    }

    [Fact]
    public void SetName_ShouldUpdateNameAndCreateEvent() {

        // Arrange
        var sut = ProductAttribute.Create("Product Attribute Name");
        var newName = "New Name";

        // Act
        sut.SetName(newName);

        // Assert
        sut.Name.Should().Be(newName);
        sut.Events.Should().ContainSingle(e => e is Events.ProductAttributeUpdated && ((Events.ProductAttributeUpdated)e).Name.Equals(newName));

    }

}
