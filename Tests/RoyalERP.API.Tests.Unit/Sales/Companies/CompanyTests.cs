using FluentAssertions;
using RoyalERP.API.Sales.Companies.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RoyalERP.API.Tests.Unit.Sales.Companies;

public class CompanyTests {

    [Fact]
    public void Create_Should_InitilizeWorkOrder() {

        // Arrange
        string name = "Company Name";

        // Act
        var company = Company.Create(name);

        // Assert
        company.Should().NotBeNull();
        company.Name.Should().Be(name);
        company.Events.Should().ContainSingle(x => ((Events.CompanyCreatedEvent)x).CompanyId == company.Id);

    }

    [Fact]
    public void Update_ShouldCreateEvent() {

        // Arrange
        var company = Company.Create("Example Company");

        string newName = "New Name";
        string newContact = "New Contact";
        string newEmail = "New Email";

        // Act
        company.Update(newName, newContact, newEmail);

        // Assert
        company.Name.Should().Be(newName);
        company.Contact.Should().Be(newContact);
        company.Email.Should().Be(newEmail);
        company.Events.Should().HaveCount(2);
        company.Events.Should().ContainSingle(x =>
            x is Events.CompanyUpdatedEvent
            && ((Events.CompanyUpdatedEvent)x).CompanyId == company.Id 
            && ((Events.CompanyUpdatedEvent)x).Name == company.Name
            && ((Events.CompanyUpdatedEvent)x).Contact == company.Contact
            && ((Events.CompanyUpdatedEvent)x).Email == company.Email
        );

    }

    [Fact]
    public void UpdateAddress_ShouldCreateEvent() {

        // Arrange
        var company = Company.Create("Example Company");

        string line1 = "Line1";
        string line2 = "Line2";
        string line3 = "Line3";
        string city = "city";
        string state = "state";
        string zip = "zip";

        // Act
        company.SetAddress(line1, line2, line3, city, state, zip);

        // Assert
        company.Address.Line1.Should().Be(line1);
        company.Address.Line2.Should().Be(line2);
        company.Address.Line3.Should().Be(line3);
        company.Address.City.Should().Be(city);
        company.Address.State.Should().Be(state);
        company.Address.Zip.Should().Be(zip);
        company.Events.Should().HaveCount(2);
        company.Events.Should().ContainSingle(x =>
            x is Events.CompanyAddressUpdatedEvent
            && ((Events.CompanyAddressUpdatedEvent)x).Line1 == company.Address.Line1
            && ((Events.CompanyAddressUpdatedEvent)x).Line2 == company.Address.Line2
            && ((Events.CompanyAddressUpdatedEvent)x).Line3 == company.Address.Line3
            && ((Events.CompanyAddressUpdatedEvent)x).City == company.Address.City
            && ((Events.CompanyAddressUpdatedEvent)x).State == company.Address.State
            && ((Events.CompanyAddressUpdatedEvent)x).Zip == company.Address.Zip
        );

    }

    [Fact]
    public void SetDefault_ShouldCreateANewDefaultConfiguration() {

        // Arrange
        var company = Company.Create("Example Company");
        var productId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var value = "Default value";

        // Act
        company.SetDefault(productId, attributeId, value);

        // Assert
        company.DefaultConfigurations.Should().ContainSingle(d => d.ProductId.Equals(productId) && d.AttributeId.Equals(attributeId) && d.Value.Equals(value));
        company.DefaultConfigurations.Should().HaveCount(1);
        company.DefaultConfigurations.First().Events.Should().Contain(e => e is Events.CompanyDefaultAddedEvent);

    }

    [Fact]
    public void SetDefault_ShouldOverwriteExisingConfiguration() {

        // Arrange
        var productId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var company = new Company(Guid.NewGuid(), 0, "", "", "", new(), new() {
            new DefaultConfiguration(Guid.NewGuid(), Guid.NewGuid(), productId, attributeId, "")
        });
        var value = "New value";

        // Act
        company.SetDefault(productId, attributeId, value);

        // Assert
        company.DefaultConfigurations.Should().ContainSingle(d => d.ProductId.Equals(productId) && d.AttributeId.Equals(attributeId) && d.Value.Equals(value));
        company.DefaultConfigurations.Should().HaveCount(1);
        company.DefaultConfigurations.First().Events.Should().Contain(e => e is Events.CompanyDefaultUpdatedEvent);

    }

    [Fact]
    public void RemoveDefault_ShouldRemoveExisingConfiguration() {

        // Arrange
        var productId = Guid.NewGuid();
        var attributeId = Guid.NewGuid();
        var config = new DefaultConfiguration(Guid.NewGuid(), Guid.NewGuid(), productId, attributeId, "");
        var company = new Company(Guid.NewGuid(), 0, "", "", "", new(), new() {
            config
        });

        // Act
        var result = company.RemoveDefault(config);

        // Assert
        result.Should().BeTrue();
        company.DefaultConfigurations.Should().HaveCount(0);
        company.Events.Should().Contain(e => e is Events.CompanyDefaultRemovedEvent);

    }

    [Fact]
    public void RemoveDefault_ShouldReturnFalseAndNotAddEvent_WhenConfigDoesNotExist() {

        // Arrange
        var company = new Company(Guid.NewGuid(), 0, "", "", "", new(), new());
        var config = new DefaultConfiguration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "");

        // Act
        var result = company.RemoveDefault(config);

        // Assert
        result.Should().BeFalse();
        company.DefaultConfigurations.Should().HaveCount(0);
        company.Events.Should().BeEmpty();

    }

    [Fact]
    public void AsDTO_ShouldCreateCorrectDTO() {

        var address = new Address() {
            Line1 = "A",
            Line2 = "A",
            Line3 = "A",
            City = "A",
            State = "A",
            Zip = "A"
        };
        var config = new DefaultConfiguration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "");
        var configs = new List<DefaultConfiguration>(){
            config
        };

        var company = new Company(Guid.NewGuid(), 123, "Company", "Contact", "email@domain", address, configs);

        var dto = company.AsDTO();
        dto.Id.Should().Be(company.Id);
        dto.Name.Should().Be(company.Name);
        dto.Email.Should().Be(company.Email);
        dto.Contact.Should().Be(company.Contact);
        dto.Address.Should().BeEquivalentTo(company.Address);
        dto.Defaults.Should().ContainSingle(d => d.AttributeId.Equals(config.AttributeId)
                                                    && d.ProductId.Equals(config.ProductId)
                                                    && d.Value.Equals(config.Value));

    }

}
