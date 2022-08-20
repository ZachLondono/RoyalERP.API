using FluentAssertions;
using RoyalERP.API.Catalog.ProductClasses.Domain;
using Xunit;

namespace RoyalERP.API.Tests.Unit.Catalog.ProductClasses;

public class ProductClassTests {

    [Fact]
    public void Create_ShouldSetNameAndCreateEvent() {

        // Arrange
        var name = "Product Class Name";

        // Act
        var sut = ProductClass.Create(name);

        // Assert
        sut.Name.Should().Be(name);
        sut.Events.Should().ContainSingle(e => e is Events.ProductClassCreated && ((Events.ProductClassCreated)e).Name.Equals(name));

    }

    [Fact]
    public void SetName_ShouldUpdateNameAndCreateEvent() {

        // Arrange
        var sut = ProductClass.Create("Product Class Name");
        var newName = "New Name";

        // Act
        sut.SetName(newName);

        // Assert
        sut.Name.Should().Be(newName);
        sut.Events.Should().ContainSingle(e => e is Events.ProductClassUpdated && ((Events.ProductClassUpdated)e).Name.Equals(newName));

    }

}
