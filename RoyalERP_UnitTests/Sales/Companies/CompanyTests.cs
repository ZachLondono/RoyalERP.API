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

}
