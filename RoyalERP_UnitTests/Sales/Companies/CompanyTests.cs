using FluentAssertions;
using RoyalERP.Sales.Companies.Domain;
using Xunit;

namespace RoyalERP_UnitTests.Sales.Companies;

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
            ((Events.CompanyUpdatedEvent)x).CompanyId == company.Id 
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
            ((Events.CompanyAddressUpdatedEvent)x).Line1 == company.Address.Line1
            && ((Events.CompanyAddressUpdatedEvent)x).Line2 == company.Address.Line2
            && ((Events.CompanyAddressUpdatedEvent)x).Line3 == company.Address.Line3
            && ((Events.CompanyAddressUpdatedEvent)x).City == company.Address.City
            && ((Events.CompanyAddressUpdatedEvent)x).State == company.Address.State
            && ((Events.CompanyAddressUpdatedEvent)x).Zip == company.Address.Zip
        );

    }

}
