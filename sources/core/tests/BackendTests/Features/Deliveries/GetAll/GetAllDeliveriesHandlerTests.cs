using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;
using Ggio.DddCore;
using Moq;
using Xunit;

namespace BackendTests.Features.Deliveries.GetAll;

public class GetAllDeliveriesHandlerTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Delivery _mockDeliveryA;
     private readonly Delivery _mockDeliveryB;

     public GetAllDeliveriesHandlerTests()
     {
          _mockDeliveryA = _fixture.Create<Delivery>();
          _mockDeliveryB = _fixture.Create<Delivery>();

          _mockDeliveryA.Code = "AAA";
          _mockDeliveryA.PackingSize = "Xxl";
          _mockDeliveryA.Urgency = "Standard";

          _mockDeliveryB.Code = "BBB";
          _mockDeliveryB.PackingSize = "Xl";
          _mockDeliveryB.Urgency = "Urgent";
     }

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
          Assert.Contains(result, delivery => delivery.ReportId == _mockDeliveryB.ReportId);
          Assert.Contains(result, delivery => delivery.Steps == _mockDeliveryA.Steps);
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
