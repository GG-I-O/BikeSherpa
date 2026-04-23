using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;
using Ggio.DddCore;
using Moq;

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
          var mockStepA1 = _fixture.Build<DeliveryStep>().With(s => s.Order, 1).Create();
          var mockStepA2 = _fixture.Build<DeliveryStep>().With(s => s.Order, 2).Create();
          _mockDeliveryA.Steps = [mockStepA1, mockStepA2];
          
          var mockStepB1 = _fixture.Build<DeliveryStep>().With(s => s.Order, 1).Create();
          var mockStepB2 = _fixture.Build<DeliveryStep>().With(s => s.Order, 2).Create();
          _mockDeliveryB.Steps = [mockStepB2, mockStepB1];
          
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
          result.Should().HaveCount(2);
          
          var deliveryA = result.Single(d => d.Code == "AAA");
          var deliveryB = result.Single(d => d.Code == "BBB");

          deliveryA.PackingSize.Should().Be("Xxl");
          deliveryA.Urgency.Should().Be("Standard");
          deliveryA.Steps.Select(s => s.Data.Order).Should().Equal(1, 2);

          deliveryB.PackingSize.Should().Be("Xl");
          deliveryB.Urgency.Should().Be("Urgent");
          deliveryB.Steps.Select(s => s.Data.Order).Should().Equal(1, 2);

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
          result.Should().NotBeNull();
          result.Should().BeEmpty();
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
