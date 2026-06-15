using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Reports.Get;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Reports.Get;

public class GetReportHandlerTests
{
     private readonly Mock<IReadRepository<Customer>> _customerRepositoryMock = new();
     private readonly Mock<IReadRepository<Delivery>> _deliveryRepositoryMock = new();
     private readonly DateTimeOffset _endDate = new(2026, 1, 31, 0, 0, 0, TimeSpan.Zero);
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly Mock<IReportService> _reportServiceMock = new();

     private readonly DateTimeOffset _startDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

     [Fact]
     public async Task Handle_ShouldReturnReport_WhenQueryIsValid()
     {
          // Arrange
          var customerId = Guid.NewGuid();

          var customer = _fixture.Build<Customer>()
               .With(c => c.Id, customerId)
               .With(c => c.Name, "Customer Name")
               .Create();

          var deliveries = _fixture.Build<Delivery>()
               .With(d => d.CustomerId, customerId)
               .With(d => d.Steps, [])
               .CreateMany(2)
               .ToList();

          var expectedReport = _fixture.Build<Report>()
               .With(r => r.CustomerName, customer.Name)
               .With(r => r.StartDate, _startDate)
               .With(r => r.EndDate, _endDate)
               .Create();

          var query = new GetReportQuery(customerId, _startDate, _endDate);
          var sut = CreateSut(customer, deliveries, expectedReport);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().Be(expectedReport);

          _reportServiceMock.Verify(
               s => s.GenerateReportAsync(
                    customer.Name,
                    _startDate,
                    _endDate,
                    deliveries),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldLoadCustomerById_ForValidationAndReportGeneration()
     {
          // Arrange
          var customerId = Guid.NewGuid();

          var customer = _fixture.Build<Customer>()
               .With(c => c.Id, customerId)
               .Create();

          var query = new GetReportQuery(customerId, _startDate, _endDate);
          var sut = CreateSut(customer, [], _fixture.Create<Report>());

          // Act
          await sut.Handle(query, CancellationToken.None);

          // Assert
          _customerRepositoryMock.Verify(
               r => r.FirstOrDefaultAsync(
                    It.Is<ISpecification<Customer>>(s => s is CustomerByIdSpecification),
                    It.IsAny<CancellationToken>()),
               Times.Exactly(2));
     }

     [Fact]
     public async Task Handle_ShouldLoadDeliveriesByCustomerAndDateRange()
     {
          // Arrange
          var customerId = Guid.NewGuid();

          var customer = _fixture.Build<Customer>()
               .With(c => c.Id, customerId)
               .Create();

          var query = new GetReportQuery(customerId, _startDate, _endDate);
          var sut = CreateSut(customer, [], _fixture.Create<Report>());

          // Act
          await sut.Handle(query, CancellationToken.None);

          // Assert
          _deliveryRepositoryMock.Verify(
               r => r.ListAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByCustomerAndDateRangeSpecification),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldPassCustomerNameDatesAndDeliveriesToReportService()
     {
          // Arrange
          var customerId = Guid.NewGuid();

          var customer = _fixture.Build<Customer>()
               .With(c => c.Id, customerId)
               .With(c => c.Name, "Report Customer")
               .Create();

          var deliveries = _fixture.Build<Delivery>()
               .With(d => d.CustomerId, customerId)
               .With(d => d.Steps, [])
               .CreateMany(3)
               .ToList();

          var expectedReport = _fixture.Create<Report>();
          var query = new GetReportQuery(customerId, _startDate, _endDate);
          var sut = CreateSut(customer, deliveries, expectedReport);

          // Act
          await sut.Handle(query, CancellationToken.None);

          // Assert
          _reportServiceMock.Verify(
               s => s.GenerateReportAsync(
                    customer.Name,
                    query.From,
                    query.To,
                    deliveries),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenCustomerIdIsEmpty()
     {
          // Arrange
          var customer = _fixture.Build<Customer>()
               .With(c => c.Id, Guid.NewGuid())
               .Create();

          var query = new GetReportQuery(Guid.Empty, _startDate, _endDate);
          var sut = CreateSut(customer, [], _fixture.Create<Report>());

          // Act
          var act = async () => await sut.Handle(query, CancellationToken.None);

          // Assert
          await act.Should().ThrowAsync<ValidationException>();

          _deliveryRepositoryMock.Verify(
               r => r.ListAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _reportServiceMock.Verify(
               s => s.GenerateReportAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<List<Delivery>>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenCustomerDoesNotExist()
     {
          // Arrange
          var query = new GetReportQuery(Guid.NewGuid(), _startDate, _endDate);
          var sut = CreateSut(null, [], _fixture.Create<Report>());

          // Act
          var act = async () => await sut.Handle(query, CancellationToken.None);

          // Assert
          await act.Should().ThrowAsync<ValidationException>()
               .WithMessage("*Customer does not exist*");

          _customerRepositoryMock.Verify(
               r => r.FirstOrDefaultAsync(
                    It.Is<ISpecification<Customer>>(s => s is CustomerByIdSpecification),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          _deliveryRepositoryMock.Verify(
               r => r.ListAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _reportServiceMock.Verify(
               s => s.GenerateReportAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<List<Delivery>>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenFromDateIsAfterToDate()
     {
          // Arrange
          var customerId = Guid.NewGuid();

          var customer = _fixture.Build<Customer>()
               .With(c => c.Id, customerId)
               .Create();

          var query = new GetReportQuery(
               customerId,
               _endDate.AddDays(1),
               _startDate);

          var sut = CreateSut(customer, [], _fixture.Create<Report>());

          // Act
          var act = async () => await sut.Handle(query, CancellationToken.None);

          // Assert
          await act.Should().ThrowAsync<ValidationException>();

          _deliveryRepositoryMock.Verify(
               r => r.ListAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _reportServiceMock.Verify(
               s => s.GenerateReportAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<List<Delivery>>()),
               Times.Never);
     }

     private GetReportHandler CreateSut(
          Customer? customer,
          List<Delivery> deliveries,
          Report report)
     {
          _customerRepositoryMock.Reset();
          _deliveryRepositoryMock.Reset();
          _reportServiceMock.Reset();

          _customerRepositoryMock
               .Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Customer>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync((ISpecification<Customer> specification, CancellationToken _) =>
                    customer is not null && specification.IsSatisfiedBy(customer)
                         ? customer
                         : null);

          _deliveryRepositoryMock
               .Setup(r => r.ListAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(deliveries);

          _reportServiceMock
               .Setup(s => s.GenerateReportAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<List<Delivery>>()))
               .ReturnsAsync(report);

          var validator = new GetReportQueryValidator(_customerRepositoryMock.Object);

          return new GetReportHandler(
               _deliveryRepositoryMock.Object,
               _customerRepositoryMock.Object,
               validator,
               _reportServiceMock.Object);
     }
}
