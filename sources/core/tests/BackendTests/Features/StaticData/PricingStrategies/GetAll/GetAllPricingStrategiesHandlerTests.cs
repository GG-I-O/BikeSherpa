using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Features.StaticData.PricingStrategies.GetAll;

namespace BackendTests.Features.StaticData.PricingStrategies.GetAll;

public class GetAllPricingStrategiesHandlerTests
{
     [Fact]
     public async Task Handle_ShouldReturnAllPricingStrategies_InTheEnum()
     {
          // Arrange
          var sut = CreateSut();
          var query = new GetAllPricingStrategiesQuery();
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          result.Should().NotBeNull();
          result.Count.Should().Be(3);
          result[(int)PricingStrategy.CustomStrategy].Value.Should().Be(PricingStrategy.CustomStrategy);
          result[(int)PricingStrategy.SimpleDeliveryStrategy].Value.Should().Be(PricingStrategy.SimpleDeliveryStrategy);
          result[(int)PricingStrategy.TourDeliveryStrategy].Value.Should().Be(PricingStrategy.TourDeliveryStrategy);
     }
     
     private GetAllPricingStrategiesHandler CreateSut()
     {
          return new GetAllPricingStrategiesHandler();
     }
}
