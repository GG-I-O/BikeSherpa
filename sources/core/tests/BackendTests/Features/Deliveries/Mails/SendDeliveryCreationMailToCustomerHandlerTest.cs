using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Mails;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Mails;

public class SendDeliveryCreationMailToCustomerHandlerTests
{
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private static SendDeliveryCreationMailToCustomerHandler MakeSut(
          out Mock<IReadRepository<Delivery>> mockDeliveryRepository,
          out Mock<IReadRepository<Customer>> mockCustomerRepository,
          out Mock<IValidator<SendDeliveryCreationMailToCustomerCommand>> mockValidator,
          out Mock<IMailService> mockMailService)
     {
          mockDeliveryRepository = new Mock<IReadRepository<Delivery>>();
          mockCustomerRepository = new Mock<IReadRepository<Customer>>();
          mockValidator = new Mock<IValidator<SendDeliveryCreationMailToCustomerCommand>>();
          mockMailService = new Mock<IMailService>();

          return new SendDeliveryCreationMailToCustomerHandler(
               mockDeliveryRepository.Object,
               mockCustomerRepository.Object,
               mockValidator.Object,
               mockMailService.Object);
     }

     [Fact]
     public async Task Handle_ShouldReturnSuccess_WhenSimpleDeliveryMailIsSentSuccessfully()
     {
          // Arrange
          var sut = MakeSut(
               out var mockDeliveryRepository,
               out var mockCustomerRepository,
               out var mockValidator,
               out var mockMailService);

          var deliveryId = _fixture.Create<Guid>();
          var command = new SendDeliveryCreationMailToCustomerCommand(deliveryId);

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Id, deliveryId)
               .With(d => d.PricingStrategy, PricingStrategy.SimpleDeliveryStrategy)
               .With(d => d.Steps, new List<DeliveryStep>())
               .Create();

          var customer = _fixture.Create<Customer>();

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<SendDeliveryCreationMailToCustomerCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockDeliveryRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(delivery);

          mockCustomerRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Customer>>(s => s is CustomerByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);

          mockMailService
               .Setup(x => x.SendSimpleDeliveryMailToCustomer(
                    It.IsAny<Delivery>(),
                    It.IsAny<Customer>()))
               .Returns(Task.CompletedTask);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          mockMailService.Verify(
               x => x.SendSimpleDeliveryMailToCustomer(delivery, customer),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldReturnSuccess_WhenTourDeliveryMailIsSentSuccessfully()
     {
          // Arrange
          var sut = MakeSut(
               out var mockDeliveryRepository,
               out var mockCustomerRepository,
               out var mockValidator,
               out var mockMailService);

          var deliveryId = _fixture.Create<Guid>();
          var command = new SendDeliveryCreationMailToCustomerCommand(deliveryId);

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Id, deliveryId)
               .With(d => d.PricingStrategy, PricingStrategy.TourDeliveryStrategy)
               .With(d => d.Steps, new List<DeliveryStep>())
               .Create();

          var customer = _fixture.Create<Customer>();

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<SendDeliveryCreationMailToCustomerCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockDeliveryRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(delivery);

          mockCustomerRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Customer>>(s => s is CustomerByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);

          mockMailService
               .Setup(x => x.SendTourDeliveryMailToCustomer(
                    It.IsAny<Delivery>(),
                    It.IsAny<Customer>()))
               .Returns(Task.CompletedTask);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          mockMailService.Verify(
               x => x.SendTourDeliveryMailToCustomer(delivery, customer),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldReturnError_WhenPricingStrategyIsCustom()
     {
          // Arrange
          var sut = MakeSut(
               out var mockDeliveryRepository,
               out var mockCustomerRepository,
               out var mockValidator,
               out var mockMailService);

          var deliveryId = _fixture.Create<Guid>();
          var command = new SendDeliveryCreationMailToCustomerCommand(deliveryId);

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Id, deliveryId)
               .With(d => d.PricingStrategy, PricingStrategy.CustomStrategy)
               .With(d => d.Steps, new List<DeliveryStep>())
               .Create();

          var customer = _fixture.Create<Customer>();

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<SendDeliveryCreationMailToCustomerCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockDeliveryRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(delivery);

          mockCustomerRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Customer>>(s => s is CustomerByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.Errors.Should().Contain("Custom strategy not implemented");
          mockMailService.Verify(
               x => x.SendSimpleDeliveryMailToCustomer(It.IsAny<Delivery>(), It.IsAny<Customer>()),
               Times.Never);

          mockMailService.Verify(
               x => x.SendTourDeliveryMailToCustomer(It.IsAny<Delivery>(), It.IsAny<Customer>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenDeliveryDoesNotExist()
     {
          // Arrange
          var sut = MakeSut(
               out var mockDeliveryRepository,
               out var mockCustomerRepository,
               out var mockValidator,
               out _);

          var deliveryId = _fixture.Create<Guid>();
          var command = new SendDeliveryCreationMailToCustomerCommand(deliveryId);

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<SendDeliveryCreationMailToCustomerCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockDeliveryRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Delivery);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          result.Errors.Should().Contain("Delivery not found");
          mockCustomerRepository.Verify(
               x => x.SingleOrDefaultAsync(
                    It.IsAny<SingleResultSpecification<Customer>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenCustomerDoesNotExist()
     {
          // Arrange
          var sut = MakeSut(
               out var mockDeliveryRepository,
               out var mockCustomerRepository,
               out var mockValidator,
               out var mockMailService);

          var deliveryId = _fixture.Create<Guid>();
          var command = new SendDeliveryCreationMailToCustomerCommand(deliveryId);

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Id, deliveryId)
               .With(d => d.PricingStrategy, PricingStrategy.SimpleDeliveryStrategy)
               .With(d => d.Steps, new List<DeliveryStep>())
               .Create();

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<SendDeliveryCreationMailToCustomerCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockDeliveryRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(delivery);

          mockCustomerRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Customer>>(s => s is CustomerByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Customer);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          result.Errors.Should().Contain("Customer not found");
          mockMailService.Verify(
               x => x.SendSimpleDeliveryMailToCustomer(It.IsAny<Delivery>(), It.IsAny<Customer>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldValidateCommand()
     {
          // Arrange
          var sut = MakeSut(
               out var mockDeliveryRepository,
               out var mockCustomerRepository,
               out var mockValidator,
               out _);

          var deliveryId = _fixture.Create<Guid>();
          var command = new SendDeliveryCreationMailToCustomerCommand(deliveryId);

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Id, deliveryId)
               .With(d => d.PricingStrategy, PricingStrategy.SimpleDeliveryStrategy)
               .With(d => d.Steps, new List<DeliveryStep>())
               .Create();

          var customer = _fixture.Create<Customer>();

          mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<ValidationContext<SendDeliveryCreationMailToCustomerCommand>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          mockDeliveryRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(delivery);

          mockCustomerRepository
               .Setup(x => x.SingleOrDefaultAsync(
                    It.Is<SingleResultSpecification<Customer>>(s => s is CustomerByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);

          // Act
          await sut.Handle(command, CancellationToken.None);

          // Assert
          mockValidator.Verify(
               x => x.ValidateAsync(
                    It.Is<ValidationContext<SendDeliveryCreationMailToCustomerCommand>>(context => context.InstanceToValidate == command),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
