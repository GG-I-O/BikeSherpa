using Ardalis.Specification;
using AutoFixture;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Add;

public class AddDeliveryHandlerTests{
     private readonly Mock<IDeliveryFactory> _mockFactory = new();
     private readonly Mock<IReadRepository<Delivery>> _mockRepository = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Fixture _fixture = new();
     private readonly Mock<IUrgencyRepository> _mockUrgencyRepository = new();

     private readonly AddDeliveryCommand _mockCommand;
     private readonly Delivery _mockDelivery;

     public AddDeliveryHandlerTests()
     {
          _mockCommand = _fixture.Create<AddDeliveryCommand>();
          _mockDelivery = _fixture.Create<Delivery>();

          _mockUrgencyRepository
               .Setup(x => x.Urgencies)
               .Returns(new List<Urgency>
               {
                    new(1, _mockCommand.Urgency, 1),
                    new(2, "Standard", 1),
                    new(3, "Urgent", 1.5)
               });

          _mockFactory
               .Setup(x => x.CreateDeliveryAsync(
                    It.IsAny<PricingStrategyEnum>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>()
                    ))
               .ReturnsAsync(_mockDelivery);
     }

     private AddDeliveryHandler CreateSut()
     {
          var validator = new AddDeliveryCommandValidator(_mockRepository.Object, _mockUrgencyRepository.Object);
          return new AddDeliveryHandler(_mockFactory.Object, validator, _mockTransaction.Object);
     }

     private void SetupRepositoryTestingIfCodeExists(bool doesCodeExist)
     {
          _mockRepository
               .Setup(x => x.AnyAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByCodeSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(doesCodeExist);
     }

     private void VerifyFactoryCalledOnce()
     {
          _mockFactory.Verify(
               x => x.CreateDeliveryAsync(
                    It.IsAny<PricingStrategyEnum>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>()),
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
          SetupRepositoryTestingIfCodeExists(false); // Tell the validator that the code does not exist
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          Assert.True(result.IsSuccess);
          Assert.Equal(_mockDelivery.Id, result.Value);
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
          Assert.True(result.IsSuccess);
          Assert.Equal(_mockDelivery.Id, result.Value);
          _mockFactory.Verify(
               x => x.CreateDeliveryAsync(
                    It.IsAny<PricingStrategyEnum>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>()),
               Times.Once);
     }
    
     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenCodeIsEmpty()
     {
          // Arrange
          var commandWithEmptyCode = _mockCommand with { Code = "" };
          SetupRepositoryTestingIfCodeExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(commandWithEmptyCode, CancellationToken.None).AsTask());

          _mockFactory.Verify(
               x => x.CreateDeliveryAsync(
                    It.IsAny<PricingStrategyEnum>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>()),
               Times.Never);

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenCodeAlreadyExists()
     {
          // Arrange
          SetupRepositoryTestingIfCodeExists(true);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(_mockCommand, CancellationToken.None).AsTask());

          _mockFactory.Verify(
               x => x.CreateDeliveryAsync(
                    It.IsAny<PricingStrategyEnum>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>()),
               Times.Never);

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldRespectCancellationToken()
     {
          // Arrange
          var cancellationTokenSource = new CancellationTokenSource();
          await cancellationTokenSource.CancelAsync();
          SetupRepositoryTestingIfCodeExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAnyAsync<Exception>(() =>
               sut.Handle(_mockCommand, cancellationTokenSource.Token).AsTask());
     }
}
