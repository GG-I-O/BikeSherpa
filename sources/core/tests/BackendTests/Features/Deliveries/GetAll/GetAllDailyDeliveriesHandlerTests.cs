using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.GetAll;

public class GetAllDailyDeliveriesHandlerTests
{
     private readonly Mock<IReadRepository<Courier>> _mockCourierRepository = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Courier _mockCourier;
     private readonly Delivery _mockDelivery;

     public GetAllDailyDeliveriesHandlerTests()
     {
          _mockCourier = _fixture.Create<Courier>();

          _mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .Create();
     }

     [Fact]
     public async Task Handle_ShouldReturnDailySteps_WhenCourierExistsAndDeliveriesExist()
     {
          // Arrange
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);
          var otherDate = date.AddDays(1);

          var stepA = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.CourierId, _mockCourier.Id)
               .With(s => s.EstimatedDeliveryDate, date.AddHours(10))
               .Create();

          var stepB = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.CourierId, _mockCourier.Id)
               .With(s => s.EstimatedDeliveryDate, date.AddHours(8))
               .Create();

          var stepForAnotherDate = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.CourierId, _mockCourier.Id)
               .With(s => s.EstimatedDeliveryDate, otherDate)
               .Create();

          var stepForAnotherCourier = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.EstimatedDeliveryDate, date.AddHours(9))
               .Create();

          _mockDelivery.Steps = [stepA, stepB, stepForAnotherDate, stepForAnotherCourier];

          var sut = CreateSut(_mockCourier, [_mockDelivery]);
          var query = new GetAllDailyDeliveriesQuery(_mockCourier.Email, date);

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

          VerifyCourierRepositoryCalledOnce();
          VerifyDeliveryRepositoryCalledOnce();
     }

     [Fact]
     public async Task Handle_ShouldReturnUnauthorized_WhenCourierDoesNotExist()
     {
          // Arrange
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);
          var sut = CreateSut(null, []);
          var query = new GetAllDailyDeliveriesQuery("missing.courier@example.com", date);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().NotBeNull();
          result.Should().BeOfType<GetAllDailyDeliveriesResult.CourierNotFound>();

          VerifyCourierRepositoryCalledOnce();

          _mockDeliveryRepository.Verify(
               repo => repo.ListAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnEmptyList_WhenNoDeliveriesExist()
     {
          // Arrange
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);
          var sut = CreateSut(_mockCourier, []);
          var query = new GetAllDailyDeliveriesQuery(_mockCourier.Email, date);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().NotBeNull();
          result.Should().BeOfType<GetAllDailyDeliveriesResult.Success>();

          var success = (GetAllDailyDeliveriesResult.Success)result;
          success.Deliveries.Should().BeEmpty();

          VerifyCourierRepositoryCalledOnce();
          VerifyDeliveryRepositoryCalledOnce();
     }

     private GetAllDailyDeliveriesHandler CreateSut(Courier? courier, List<Delivery> deliveries)
     {
          _mockCourierRepository
               .Setup(repo => repo.FirstOrDefaultAsync(
                    It.Is<ISpecification<Courier>>(s => s is CourierByEmailSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(courier);

          _mockDeliveryRepository
               .Setup(repo => repo.ListAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryStepByCourierAndDate),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(deliveries);

          return new GetAllDailyDeliveriesHandler(
               _mockCourierRepository.Object,
               _mockDeliveryRepository.Object);
     }

     private void VerifyCourierRepositoryCalledOnce()
     {
          _mockCourierRepository.Verify(
               repo => repo.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Courier>>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
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