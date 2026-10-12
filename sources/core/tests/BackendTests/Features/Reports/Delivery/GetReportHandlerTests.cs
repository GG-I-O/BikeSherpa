using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Reports.Delivery;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Ggio.DddCore;
using Moq;
using CustomerEntity = Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Customer;
using DeliveryEntity = Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Delivery;

namespace BackendTests.Features.Reports.Delivery;

public class GetReportHandlerTests
{
     private readonly Mock<IReadRepository<CustomerEntity>> _customerRepositoryMock = new();
     private readonly Mock<IReadRepository<DeliveryEntity>> _deliveryRepositoryMock = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly Mock<IReportService> _reportServiceMock = new();

     private readonly DateTimeOffset _startDate = new(2026, 1, 15, 8, 30, 0, TimeSpan.Zero);

     [Fact]
     public async Task Handle_ShouldReturnReport_WhenQueryIsValid()
     {
          // Arrange
          var (customer, delivery) = CreateCustomerAndDelivery();

          var expectedReport = _fixture.Build<Report>()
               .With(r => r.CustomerName, customer.Name)
               .With(r => r.StartDate, _startDate)
               .With(r => r.EndDate, _startDate)
               .Create();

          var query = new GetReportQuery(delivery.Id);
          var sut = CreateSut(customer, delivery, expectedReport);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().Be(expectedReport);

          _reportServiceMock.Verify(
               s => s.GenerateDeliveryReportAsync(
                    customer.Name,
                    _startDate,
                    _startDate,
                    It.Is<List<DeliveryEntity>>(l => l.Count == 1 && ReferenceEquals(l[0], delivery))),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldLoadDeliveryById_ForValidationAndReportGeneration()
     {
          // Arrange
          var (customer, delivery) = CreateCustomerAndDelivery();
          var query = new GetReportQuery(delivery.Id);
          var sut = CreateSut(customer, delivery, _fixture.Create<Report>());

          // Act
          await sut.Handle(query, CancellationToken.None);

          // Assert
          _deliveryRepositoryMock.Verify(
               r => r.FirstOrDefaultAsync(
                    It.Is<ISpecification<DeliveryEntity>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()),
               Times.Exactly(2));
     }

     [Fact]
     public async Task Handle_ShouldLoadCustomerById_UsingTheDeliveryCustomerId()
     {
          // Arrange
          var (customer, delivery) = CreateCustomerAndDelivery();
          var query = new GetReportQuery(delivery.Id);
          var sut = CreateSut(customer, delivery, _fixture.Create<Report>());

          // Act
          await sut.Handle(query, CancellationToken.None);

          // Assert
          _customerRepositoryMock.Verify(
               r => r.FirstOrDefaultAsync(
                    It.Is<ISpecification<CustomerEntity>>(s => s is CustomerByIdSpecification),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldPassCustomerNameDeliveryStartDateAndDeliveryToReportService()
     {
          // Arrange
          var (customer, delivery) = CreateCustomerAndDelivery("Report Customer");
          var expectedReport = _fixture.Create<Report>();
          var query = new GetReportQuery(delivery.Id);
          var sut = CreateSut(customer, delivery, expectedReport);

          // Act
          await sut.Handle(query, CancellationToken.None);

          // Assert
          _reportServiceMock.Verify(
               s => s.GenerateDeliveryReportAsync(
                    "Report Customer",
                    delivery.StartDate,
                    delivery.StartDate,
                    It.Is<List<DeliveryEntity>>(l => l.Count == 1 && ReferenceEquals(l[0], delivery))),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenDeliveryIdIsEmpty()
     {
          // Arrange
          var (customer, delivery) = CreateCustomerAndDelivery();
          var query = new GetReportQuery(Guid.Empty);
          var sut = CreateSut(customer, delivery, _fixture.Create<Report>());

          // Act
          var act = async () => await sut.Handle(query, CancellationToken.None);

          // Assert
          await act.Should().ThrowAsync<ValidationException>();

          _customerRepositoryMock.Verify(
               r => r.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<CustomerEntity>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _reportServiceMock.Verify(
               s => s.GenerateDeliveryReportAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<List<DeliveryEntity>>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenDeliveryDoesNotExist()
     {
          // Arrange
          var (customer, _) = CreateCustomerAndDelivery();
          var query = new GetReportQuery(Guid.NewGuid());
          var sut = CreateSut(customer, null, _fixture.Create<Report>());

          // Act
          var act = async () => await sut.Handle(query, CancellationToken.None);

          // Assert
          await act.Should().ThrowAsync<ValidationException>()
               .WithMessage("*Delivery does not exist*");

          _deliveryRepositoryMock.Verify(
               r => r.FirstOrDefaultAsync(
                    It.Is<ISpecification<DeliveryEntity>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          _customerRepositoryMock.Verify(
               r => r.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<CustomerEntity>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _reportServiceMock.Verify(
               s => s.GenerateDeliveryReportAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<List<DeliveryEntity>>()),
               Times.Never);
     }

     private (CustomerEntity Customer, DeliveryEntity Delivery) CreateCustomerAndDelivery(string customerName = "Customer Name")
     {
          var customerId = Guid.NewGuid();

          var customer = _fixture.Build<CustomerEntity>()
               .With(c => c.Id, customerId)
               .With(c => c.Name, customerName)
               .Create();

          var delivery = _fixture.Build<DeliveryEntity>()
               .With(d => d.Id, Guid.NewGuid())
               .With(d => d.CustomerId, customerId)
               .With(d => d.StartDate, _startDate)
               .With(d => d.Steps, [])
               .Create();

          return (customer, delivery);
     }

     private GetReportHandler CreateSut(
          CustomerEntity? customer,
          DeliveryEntity? delivery,
          Report report)
     {
          _customerRepositoryMock.Reset();
          _deliveryRepositoryMock.Reset();
          _reportServiceMock.Reset();

          _customerRepositoryMock
               .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<CustomerEntity>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync((ISpecification<CustomerEntity> specification, CancellationToken _) =>
                    customer is not null && specification.IsSatisfiedBy(customer)
                         ? customer
                         : null);

          _deliveryRepositoryMock
               .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<DeliveryEntity>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync((ISpecification<DeliveryEntity> specification, CancellationToken _) =>
                    delivery is not null && specification.IsSatisfiedBy(delivery)
                         ? delivery
                         : null);

          _reportServiceMock
               .Setup(s => s.GenerateDeliveryReportAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<List<DeliveryEntity>>()))
               .ReturnsAsync(report);

          var validator = new GetReportQueryValidator(_deliveryRepositoryMock.Object);

          return new GetReportHandler(
               _deliveryRepositoryMock.Object,
               _customerRepositoryMock.Object,
               validator,
               _reportServiceMock.Object);
     }
}