using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.GetAll;

public class GetAllDeliveriesHandlerTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockRepository = new();

     private readonly Delivery _mockDeliveryA = new()
     {
          Id = Guid.NewGuid(),
          Code = "AAA",
          CustomerId = Guid.NewGuid(),
          PackingSize = "Xxl",
          PricingStrategy = PricingStrategyEnum.CustomStrategy,
          Urgency = "Standard",
          ReportId = Guid.NewGuid(),
          Steps = [],
          InsulatedBox = true,
          ExactTime = false,
          ContractDate = new DateTimeOffset(2026, 01, 01, 0, 0, 0, TimeSpan.Zero),
          StartDate = new DateTimeOffset(2026, 01, 01, 2, 10, 0, TimeSpan.Zero)
     };
     
     private readonly Delivery _mockDeliveryB = new ()
     {
          Id = Guid.NewGuid(),
          Code = "BBB",
          CustomerId = Guid.NewGuid(),
          PackingSize = "Xl",
          PricingStrategy = PricingStrategyEnum.CustomStrategy,
          Urgency = "Urgent",
          ReportId = Guid.NewGuid(),
          Steps = [],
          InsulatedBox = true,
          ExactTime = false,
          ContractDate = new DateTimeOffset(2026, 02, 03, 0, 0, 0, TimeSpan.Zero),
          StartDate = new DateTimeOffset(2026, 02, 04, 2, 5, 0, TimeSpan.Zero)
     };

     [Fact]
     public async Task Handle_ShouldReturnAllDeliveries_WhenDeliveriesExist()
     {
          // Arrange
          var deliveries = new List<Delivery>
          {
               _mockDeliveryA,
               _mockDeliveryB
          };

          var sut = CreateSut(deliveries);
          var query = new GetAllDeliveriesQuery(null);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          Assert.NotNull(result);
          Assert.Equal(2, result.Count);
          Assert.Contains(result, delivery => delivery.Code == "BBB");
          Assert.Contains(result, delivery => delivery.Code == "AAA");
          Assert.Contains(result, delivery => delivery.PackingSize == "Xxl");
          Assert.Contains(result, delivery => delivery.PackingSize == "Xl");
          Assert.Contains(result, delivery => delivery.Urgency == "Standard");
          Assert.Contains(result, delivery => delivery.Urgency == "Urgent");
          Assert.Contains(result, delivery => delivery.ReportId == _mockDeliveryA.ReportId);
          Assert.Contains(result, delivery => delivery.ReportId == _mockDeliveryB.ReportId);
          Assert.Contains(result, delivery => delivery.Steps == _mockDeliveryA.Steps);
          Assert.Contains(result, delivery => delivery.Steps == _mockDeliveryB.Steps);
          VerifyRepositoryCalledOnce();
     }

     [Fact]
     public async Task Handle_ShouldReturnEmptyList_WhenNoDeliveriesExist()
     {
          // Arrange
          var sut = CreateSut(new List<Delivery>());
          var query = new GetAllDeliveriesQuery(null);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          Assert.NotNull(result);
          Assert.Empty(result);
          VerifyRepositoryCalledOnce();
     }

     private GetAllDeliveriesHandler CreateSut(List<Delivery> returnDeliveries)
     {
          _mockRepository
               .Setup(repo => repo.ListAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(returnDeliveries);

          return new GetAllDeliveriesHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(
               repo => repo.ListAsync(It.IsAny<CancellationToken>()),
               Times.Once
          );
     }
}
