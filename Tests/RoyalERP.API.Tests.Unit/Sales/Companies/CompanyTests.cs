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

        // Act
        company.Update(newName);

        // Assert
        company.Name.Should().Be(newName);
        company.Events.Should().HaveCount(2);
        company.Events.Should().ContainSingle(x =>
            x is Events.CompanyNameUpdatedEvent
            && ((Events.CompanyNameUpdatedEvent)x).CompanyId == company.Id 
            && ((Events.CompanyNameUpdatedEvent)x).Name == company.Name
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
        var company = new Company(Guid.NewGuid(), 0, "", new(), new() {
            new DefaultConfiguration(Guid.NewGuid(), Guid.NewGuid(), productId, attributeId, "")
        }, new(), new());
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
        var company = new Company(Guid.NewGuid(), 0, "", new(), new() {
            config
        }, new(), new());

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
        var company = new Company(Guid.NewGuid(), 0, "", new(), new(), new(), new());
        var config = new DefaultConfiguration(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "");

        // Act
        var result = company.RemoveDefault(config);

        // Assert
        result.Should().BeFalse();
        company.DefaultConfigurations.Should().HaveCount(0);
        company.Events.Should().BeEmpty();

    }

    [Fact]
    public void SetInfo_ShouldAddInfo() {

        // Arrange
        var company = new Company(Guid.NewGuid(), 0, "", new(), new(), new(), new());
        string field = "Key1";
        string value = "Value1";

        // Act
        company.SetInfo(field, value);

        // Assert
        company.Info.Should().Contain(new KeyValuePair<string, string>(field, value));
        company.Events.Should().Contain(e => e is Events.CompanyInfoFieldSetEvent);

    }

    [Fact]
    public void RemoveInfo_ShouldRemoveInfo() {

        // Arrange
        string field = "Key1";
        string value = "Value1";
        var company = new Company(Guid.NewGuid(), 0, "", new(), new(), new() {
            { field, value}
        }, new());

        // Act
        var result = company.RemoveInfo(field);

        // Assert
        result.Should().Be(true);
        company.Info.Should().NotContain(new KeyValuePair<string, string>(field, value));
        company.Events.Should().Contain(e => e is Events.CompanyInfoFieldRemovedEvent);

    }

    [Fact]
    public void RemoveInfo_ShouldNotRemoveInfo_WhenFieldDoesNotExist() {

        // Arrange
        string field = "Key1";
        string value = "Value1";
        var company = new Company(Guid.NewGuid(), 0, "", new(), new(), new() {
            { field, value}
        }, new());

        // Act
        var result = company.RemoveInfo("Key2");

        // Assert
        result.Should().Be(false);
        company.Info.Should().Contain(new KeyValuePair<string, string>(field, value));
        company.Events.Should().BeEmpty();

    }

    [Fact]
    public void AddContact_ShouldAddToContactsCollection() {

        // Arrange
        var company = new Company(Guid.NewGuid(), 123, "Company", new(), new(), new(), new());
        string contactName = "Name";
        string contactEmail = "Email";
        string contactPhone = "Phone";
        List<string> contactRoles = new() {
            "owner"
        };

        // Act 
        var contact = company.AddContact(contactName, contactEmail, contactPhone, contactRoles);

        // Assert
        company.Contacts.Should().HaveCount(1);
        contact.Name.Should().Be(contactName);
        contact.Email.Should().Be(contactEmail);
        contact.Phone.Should().Be(contactPhone);
        contact.Roles.Should().ContainSingle("owner");
        company.Contacts.Should().ContainSingle(c => c.Name == contactName && c.Email == contactEmail && c.Phone == contactPhone && c.Roles.Contains("owner"));

    }

    [Fact]
    public void RemoveContact_ShouldRemoveFromContactsCollection() {

        // Arrange
        var company = new Company(Guid.NewGuid(), 123, "Company", new(), new(), new(), new());
        string contactName = "Name";
        string contactEmail = "Email";
        string contactPhone = "Phone";
        List<string> contactRoles = new() {
            "owner"
        };

        // Act
        var contact = company.AddContact(contactName, contactEmail, contactPhone, contactRoles);
        var result = company.RemoveContact(contact);

        // Assert
        company.Contacts.Should().HaveCount(0);
        result.Should().BeTrue();

    }

    [Fact]
    public void AddRole_ShouldAddToRoleCollection() {

        // Arrange
        var contact = new Contact(Guid.NewGuid(), Guid.NewGuid(), "name", "email", "phone", new());
        var roleA = "owner";
        var roleB = "billing";

        // Act
        contact.AddRole(roleA);
        contact.AddRole(roleB);

        // Assert
        contact.Roles.Should().HaveCount(2);
        contact.Roles.Should().Contain(roleA);
        contact.Roles.Should().Contain(roleB);

    }

    [Fact]
    public void AddRole_ShouldNotAddDuplicatesToRoleCollection() {

        // Arrange
        var role = "owner";
        var contact = new Contact(Guid.NewGuid(), Guid.NewGuid(), "name", "email", "phone", new() { role });

        // Act
        contact.AddRole(role);

        // Assert
        contact.Roles.Should().HaveCount(1);
        contact.Roles.Should().ContainSingle(role);

    }

    [Fact]
    public void RemoveRole_ShouldRemoveFromRoleCollection() {

        // Arrange
        var role = "owner";
        var contact = new Contact(Guid.NewGuid(), Guid.NewGuid(), "name", "email", "phone", new() { role });

        // Act
        var result = contact.RemoveRole(role);

        // Assert
        contact.Roles.Should().HaveCount(0);
        result.Should().BeTrue();

    }


    [Fact]
    public void RemoveRole_ShouldReturnFalseWhenRoleDoesNotExist() {

        // Arrange
        var role = "owner";
        var doesNotExist = "abc123";
        var contact = new Contact(Guid.NewGuid(), Guid.NewGuid(), "name", "email", "phone", new() { role });

        // Act
        var result = contact.RemoveRole(doesNotExist);

        // Assert
        contact.Roles.Should().ContainSingle(role);
        contact.Roles.Should().HaveCount(1);
        result.Should().BeFalse();

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

        var info = new Dictionary<string, string>() {
            { "key1", "value1"}
        };

        var contact = new Contact(Guid.NewGuid(), Guid.NewGuid(), "name", "email", "phone", new());
        var contacts = new List<Contact>() {
            contact
        };

        var company = new Company(Guid.NewGuid(), 123, "Company", address, configs, info, contacts);

        var dto = company.AsDTO();
        dto.Id.Should().Be(company.Id);
        dto.Name.Should().Be(company.Name);
        dto.Address.Should().BeEquivalentTo(company.Address);
        dto.Defaults.Should().ContainSingle(d => d.AttributeId.Equals(config.AttributeId)
                                                    && d.ProductId.Equals(config.ProductId)
                                                    && d.Value.Equals(config.Value));
        dto.Info.Should().Contain(new KeyValuePair<string, string>("key1", "value1"));
        dto.Contacts.Should().ContainSingle(c => c.Id == contact.Id && c.Name == contact.Name && c.Email == contact.Email && c.Phone == contact.Phone);

    }

}
