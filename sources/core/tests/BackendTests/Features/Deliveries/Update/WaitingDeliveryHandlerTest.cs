using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Update;

public class WaitingDeliveryHandlerTests
{
     private readonly WaitingDeliveryCommand _command;
     private readonly Delivery _delivery;
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly Mock<IReadRepository<Delivery>> _mockReadRepository = new();
     private readonly Mock<IValidator<WaitingDeliveryCommand>> _mockValidator = new();


     public WaitingDeliveryHandlerTests()
     {
          _command = _fixture.Create<WaitingDeliveryCommand>();

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.Status, DeliveryStatus.New)
               .Create();

          _mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<WaitingDeliveryCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          _mockReadRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_delivery);
     }

     private static WaitingDeliveryHandler MakeSut(
          out Mock<IReadRepository<Delivery>> mockReadRepository,
          out Mock<IValidator<WaitingDeliveryCommand>> mockValidator,
          out Mock<IApplicationTransaction> mockApplicationTransaction)
     {
          mockReadRepository = new Mock<IReadRepository<Delivery>>();
          mockValidator = new Mock<IValidator<WaitingDeliveryCommand>>();
          mockApplicationTransaction = new Mock<IApplicationTransaction>();

          return new WaitingDeliveryHandler(
               mockReadRepository.Object,
               mockValidator.Object,
               mockApplicationTransaction.Object);
     }

     [Fact]
     public async Task Handle_ShouldReturnSuccess_WhenCommandIsValidAndDeliveryExists()
     {
          // Arrange
          var sut = MakeSut(
               out var mockReadRepository,
               out var mockValidator,
               out _);

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<WaitingDeliveryCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockReadRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.IsAny<SingleResultSpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_delivery);

          // Act
          var result = await sut.Handle(_command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenDeliveryDoesNotExist()
     {
          // Arrange
          var sut = MakeSut(
               out var mockReadRepository,
               out var mockValidator,
               out _);

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<WaitingDeliveryCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockReadRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.IsAny<SingleResultSpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Delivery);

          // Act
          var result = await sut.Handle(_command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
     }

     [Fact]
     public async Task Handle_ShouldCallWaitingOnDelivery_WhenDeliveryExists()
     {
          // Arrange
          var sut = MakeSut(
               out var mockReadRepository,
               out var mockValidator,
               out _);

          var mockDelivery = _fixture.Build<Delivery>()
               .With(s => s.Status, DeliveryStatus.New)
               .Without(s => s.Steps)
               .Create();

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<WaitingDeliveryCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockReadRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.IsAny<SingleResultSpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(mockDelivery);

          // Act
          await sut.Handle(_command, CancellationToken.None);

          // Assert
          mockDelivery.Status.Should().Be(DeliveryStatus.Pending);
     }

     [Fact]
     public async Task Handle_ShouldCommitTransaction_WhenDeliveryIsWaitingSuccessfully()
     {
          // Arrange
          var sut = MakeSut(
               out var mockReadRepository,
               out var mockValidator,
               out var mockApplicationTransaction);

          var mockDelivery = _fixture.Build<Delivery>()
               .With(s => s.Status, DeliveryStatus.New)
               .Without(s => s.Steps)
               .Create();

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<WaitingDeliveryCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockReadRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.IsAny<SingleResultSpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(mockDelivery);

          // Act
          await sut.Handle(_command, CancellationToken.None);

          // Assert
          mockApplicationTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldNotCommitTransaction_WhenDeliveryDoesNotExist()
     {
          // Arrange
          var sut = MakeSut(
               out var mockReadRepository,
               out var mockValidator,
               out var mockApplicationTransaction);

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<WaitingDeliveryCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockReadRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.IsAny<SingleResultSpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Delivery);

          // Act
          await sut.Handle(_command, CancellationToken.None);

          // Assert
          mockApplicationTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldUseCorrectSpecification_WhenFetchingDelivery()
     {
          // Arrange
          var sut = MakeSut(
               out var mockReadRepository,
               out var mockValidator,
               out _);

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<WaitingDeliveryCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockReadRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.IsAny<SingleResultSpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_delivery);

          // Act
          await sut.Handle(_command, CancellationToken.None);

          // Assert
          mockReadRepository.Verify(
               x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
