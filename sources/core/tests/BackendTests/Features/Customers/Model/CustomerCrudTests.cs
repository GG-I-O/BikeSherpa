using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Model;

namespace BackendTests.Features.Customers.Model;

public class CustomerCrudTests
{
    [Fact]
    public void CustomerCrud_CanBeCreatedAndUsed()
    {
        // Arrange
        var fixture = new Fixture();

        // Act
        var customerCrud = fixture.Create<CustomerCrud>();

        // Assert
        customerCrud.Should().NotBeNull();
        customerCrud.Id.Should().NotBe(Guid.Empty);
        customerCrud.Name.Should().NotBeNullOrEmpty();
        customerCrud.Address.Should().NotBeNull();

        // Verify type
        var type = typeof(CustomerCrud);
        type.Should().NotBeNull();
    }
}
