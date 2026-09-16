using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.GetAll;

public class GetAllUnassignedDeliveriesHandlerTests
{
     private readonly Mock<IReadRepository<Courier>> _mockCourierRepository = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Courier _mockCourier;
     private readonly Delivery _mockDelivery;

     public GetAllUnassignedDeliveriesHandlerTests()
     {
          _mockCourier = _fixture.Create<Courier>();

          _mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .Create();
     }

     [Fact]
     public async Task Handle_ShouldReturnDailySteps_WhenUnassignedDeliveriesExist()
     {
          // Arrange
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);
          var otherDate = date.AddDays(1);

          var stepA = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.CourierId, (Guid?)null)
               .With(s => s.EstimatedDeliveryDate, date.AddHours(10))
               .Create();

          var stepB = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.CourierId, (Guid?)null)
               .With(s => s.EstimatedDeliveryDate, date.AddHours(8))
               .Create();

          var stepForAnotherDate = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.CourierId, _mockCourier.Id)
               .With(s => s.EstimatedDeliveryDate, otherDate)
               .Create();

          var stepForAnotherCourier = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.CourierId, _mockCourier.Id)
               .With(s => s.EstimatedDeliveryDate, date.AddHours(9))
               .Create();

          _mockDelivery.Steps = [stepA, stepB, stepForAnotherDate, stepForAnotherCourier];

          var sut = CreateSut([_mockDelivery]);
          var query = new GetAllUnassignedDeliveriesQuery(date);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().NotBeNull();
          result.Should().BeOfType<GetAllDailyDeliveriesResult.Success>();

          var success = (GetAllDailyDeliveriesResult.Success)result;
          success.Deliveries.Should().HaveCount(1);

          var delivery = success.Deliveries.Single();
          delivery.Id.Should().Be(_mockDelivery.Id);
          delivery.Steps.Should().HaveCount(2);
          delivery.Steps.Select(s => s.Data.Id).Should().Equal(stepB.Id, stepA.Id);

          VerifyDeliveryRepositoryCalledOnce();
     }

     [Fact]
     public async Task Handle_ShouldReturnEmptyList_WhenNoDeliveriesExist()
     {
          // Arrange
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);
          var sut = CreateSut([]);
          var query = new GetAllUnassignedDeliveriesQuery(date);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().NotBeNull();
          result.Should().BeOfType<GetAllDailyDeliveriesResult.Success>();

          var success = (GetAllDailyDeliveriesResult.Success)result;
          success.Deliveries.Should().BeEmpty();

          VerifyDeliveryRepositoryCalledOnce();
     }

     private GetAllUnassignedDeliveriesHandler CreateSut(List<Delivery> deliveries)
     {
          _mockDeliveryRepository
               .Setup(repo => repo.ListAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryStepByCourierAndDate),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(deliveries);

          return new GetAllUnassignedDeliveriesHandler(
               _mockDeliveryRepository.Object);
     }

     private void VerifyDeliveryRepositoryCalledOnce()
     {
          _mockDeliveryRepository.Verify(
               repo => repo.ListAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}