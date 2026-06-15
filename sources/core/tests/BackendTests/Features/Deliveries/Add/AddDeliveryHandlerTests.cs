using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Add;

public class AddDeliveryHandlerTests
{
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly AddDeliveryCommand _mockCommand;
     private readonly Delivery _mockDelivery;
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IDeliveryFactory> _mockFactory = new();
     private readonly Mock<IPackingSizeRepository> _mockPackingSizeRepository = new();
     private readonly Mock<IPricingStrategyService> _mockPricingStrategyService = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IUrgencyRepository> _mockUrgencyRepository = new();

     public AddDeliveryHandlerTests()
     {
          var mockCustomer = _fixture.Create<Customer>();
          _mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .Create();

          _mockDelivery.GenerateReportId(mockCustomer);
          _mockPricingStrategyService.Setup(s => s.CalculateDeliveryPriceWithoutVat(_mockDelivery))
               .ReturnsAsync(10.0);
          _mockDelivery.TotalPrice = 10.0;

          var urgencies = new List<Urgency>(_fixture.CreateMany<Urgency>(2));
          _mockUrgencyRepository
               .Setup(x => x.GetAll())
               .Returns(urgencies);

          _mockUrgencyRepository
               .Setup(x => x.GetByName(It.IsAny<string>()))
               .Returns(urgencies[0]);

          var packingSizes = new List<PackingSize>(_fixture.CreateMany<PackingSize>(2));
          _mockPackingSizeRepository
               .Setup(x => x.GetAll())
               .Returns(packingSizes);

          _mockPackingSizeRepository
               .Setup(x => x.GetByName(It.IsAny<string>()))
               .Returns(packingSizes[0]);

          _mockCommand = _fixture.Build<AddDeliveryCommand>()
               .With(c => c.Urgency, urgencies[0].Name)
               .Create();

          _mockFactory
               .Setup(x => x.CreateDeliveryAsync(It.IsAny<DeliveryFactoryParameters>()
               ))
               .ReturnsAsync(_mockDelivery);
     }

     private AddDeliveryHandler CreateSut()
     {
          var validator = new AddDeliveryCommandValidator(_mockUrgencyRepository.Object);
          return new AddDeliveryHandler(_mockFactory.Object, _mockUrgencyRepository.Object, validator, _mockTransaction.Object);
     }

     private void SetupRepositoryTestingIfCodeExists(bool doesCodeExist)
     {
          _mockDeliveryRepository
               .Setup(x => x.AnyAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByCodeSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(doesCodeExist);
     }

     private void VerifyFactoryCalledOnce()
     {
          _mockFactory.Verify(
               x => x.CreateDeliveryAsync(It.IsAny<DeliveryFactoryParameters>()),
               Times.Once);
     }

     private void VerifyTransactionCommittedOnce()
     {
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldCreateDeliveryAndReturnId_WhenCommandIsValid()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(false);
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          result.Value.Should().Be(_mockDelivery.Id);
          VerifyFactoryCalledOnce();
          VerifyTransactionCommittedOnce();
     }

     [Fact]
     public async Task Handle_ShouldCreateDeliveryWithCorrectInfos()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(false);
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          result.Value.Should().Be(_mockDelivery.Id);
          _mockFactory.Verify(
               x => x.CreateDeliveryAsync(It.IsAny<DeliveryFactoryParameters>()),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldRespectCancellationToken()
     {
          // Arrange
          using var cancellationTokenSource = new CancellationTokenSource();
          await cancellationTokenSource.CancelAsync();
          SetupRepositoryTestingIfCodeExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAnyAsync<Exception>(() =>
               sut.Handle(_mockCommand, cancellationTokenSource.Token).AsTask());
     }
}
