using Ardalis.Specification;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Get;

public class GetDeliveryHandlerTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockRepository = new();

     private readonly Delivery _mockDelivery = new()
     {
          Id = Guid.NewGuid(),
          Code = "AAA",
          PricingStrategy = PricingStrategyEnum.CustomStrategy,
          CustomerId = Guid.Empty,
          Urgency = "Standard",
          ReportId = Guid.Empty,
          Steps = [],
          PackingSize = "Xl",
          InsulatedBox = false,
          ExactTime = false,
          ContractDate = DateTimeOffset.Now,
          StartDate = DateTimeOffset.Now + TimeSpan.FromDays(1)
     };

     [Fact]
     public async Task Handle_ShouldReturnOneDelivery_WhenDeliveryExists()
     {
          // Arrange
          var guid = Guid.NewGuid();
          _mockDelivery.Id = guid;
          var sut = CreateSut(_mockDelivery);
          var query = new GetDeliveryQuery(guid);
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          result.Should().NotBeNull();
          result.Id.Should().Be(guid);
          result.Code.Should().Be("AAA");
          result.PricingStrategy.Should().Be(PricingStrategyEnum.CustomStrategy);
          result.PackingSize.Should().Be("Xl");
          VerifyRepositoryCalledOnce();
     }
     
     [Theory]
     [InlineData(true)]
     [InlineData(false)]
     public async Task Handle_ShouldReturnNull_WhenDeliveryDoesNotExist(bool emptyRepository)
     {
          // Arrange
          var guidA = Guid.NewGuid();
          var guidB = Guid.NewGuid();
          _mockDelivery.Id = guidA;
          var sut = CreateSut(emptyRepository ? null : _mockDelivery);
          var query = new GetDeliveryQuery(guidB);
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          result.Should().BeNull();
          VerifyRepositoryCalledOnce();
     }

     private GetDeliveryHandler CreateSut(Delivery? existingDelivery)
     {
          _mockRepository
               .Setup(repo => repo.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification && existingDelivery != null && s.IsSatisfiedBy(existingDelivery)), 
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(existingDelivery);
          return new GetDeliveryHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(repo => repo.FirstOrDefaultAsync(
               It.IsAny<ISpecification<Delivery>>(), 
               It.IsAny<CancellationToken>()),
               Times.Once
          );
     }
}
