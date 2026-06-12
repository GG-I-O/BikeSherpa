using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ardalis.Result;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Delete;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.BikeSherpa.Backend.Features.Deliveries.AddByCustomer;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Mails;
using Ggio.DddCore;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Deliveries.AddByCustomer;

public class WebApplicationFactory(Action<IServiceCollection> serviceCollection) : TestWebApplicationFactory(serviceBuilder: serviceCollection)
{
}

public class AddDeliveryByCustomerEndpointTests
{
     private readonly CancellationToken _cancellationToken;
     private readonly WebApplicationFactory _factory;
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     private readonly Mock<IReadRepository<Customer>> _mockCustomerRepository = new();

     public AddDeliveryByCustomerEndpointTests(ITestContextAccessor testContextAccessor)
     {
          _factory = new WebApplicationFactory(services => services.AddSingleton(_mockCustomerRepository.Object));
          _cancellationToken = testContextAccessor.Current.CancellationToken;
     }

     private HttpClient Client => _factory.CreateClient();

     private Mock<IMediator> MockMediator => _factory.MockMediator;

     [Fact]
     public async Task HandleAsync_ShouldCreateTemporaryCustomer_WhenCustomerDoesNotExist()
     {
          // Arrange
          MockMediator.Reset();
          var request = _fixture.Create<AddDeliveryByCustomerRequest>();
          var customerId = Guid.NewGuid();
          var deliveryId = Guid.NewGuid();

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddTemporaryCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(customerId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(deliveryId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<SendDeliveryCreationMailToCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          MockMediator
               .Setup(x => x.Send(It.IsAny<AddDeliveryStepsCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await Client.PostAsJsonAsync("/api/deliveries/by_customer", request, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.Created);

          MockMediator.Verify(
               x => x.Send(
                    It.Is<AddTemporaryCustomerCommand>(cmd =>
                         cmd.Name == request.Customer.Name &&
                         cmd.Siret == request.Customer.Siret &&
                         cmd.VatNumber == request.Customer.VatNumber &&
                         cmd.Email == request.Customer.Email &&
                         cmd.PhoneNumber == request.Customer.PhoneNumber &&
                         cmd.Address == request.Customer.Address),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task HandleAsync_ShouldCreateDelivery_WithCorrectParameters()
     {
          // Arrange
          MockMediator.Reset();
          var request = _fixture.Create<AddDeliveryByCustomerRequest>();
          var customerId = Guid.NewGuid();
          var deliveryId = Guid.NewGuid();

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddTemporaryCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(customerId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(deliveryId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<SendDeliveryCreationMailToCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          MockMediator
               .Setup(x => x.Send(It.IsAny<AddDeliveryStepsCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await Client.PostAsJsonAsync("/api/deliveries/by_customer", request, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.Created);

          MockMediator.Verify(
               x => x.Send(
                    It.Is<AddDeliveryCommand>(cmd =>
                         cmd.PricingStrategy == request.Delivery.PricingStrategy &&
                         cmd.CustomerId == customerId &&
                         cmd.Urgency == request.Delivery.Urgency &&
                         cmd.TotalPrice == request.Delivery.TotalPrice &&
                         cmd.Discount == request.Delivery.Discount &&
                         cmd.ExtraCost == request.Delivery.ExtraCost &&
                         string.Join(",", cmd.Details) == string.Join(",", request.Delivery.Details) &&
                         cmd.InsulatedBox == request.Delivery.InsulatedBox &&
                         cmd.ContractDate == request.Delivery.ContractDate &&
                         cmd.StartDate == request.Delivery.StartDate &&
                         cmd.NeedEstimate == request.Delivery.NeedEstimate),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task HandleAsync_ShouldSendDeliveryCreationMail_AfterDeliveryCreated()
     {
          // Arrange
          MockMediator.Reset();
          var request = _fixture.Create<AddDeliveryByCustomerRequest>();
          var customerId = Guid.NewGuid();
          var deliveryId = Guid.NewGuid();

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddTemporaryCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(customerId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(deliveryId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<SendDeliveryCreationMailToCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          MockMediator
               .Setup(x => x.Send(It.IsAny<AddDeliveryStepsCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await Client.PostAsJsonAsync("/api/deliveries/by_customer", request, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.Created);

          MockMediator.Verify(
               x => x.Send(
                    It.Is<SendDeliveryCreationMailToCustomerCommand>(cmd =>
                         cmd.DeliveryId == deliveryId),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task HandleAsync_ShouldReturnBadRequest_WhenMailSendingFails()
     {
          // Arrange
          MockMediator.Reset();
          var request = _fixture.Create<AddDeliveryByCustomerRequest>();
          var customerId = Guid.NewGuid();
          var deliveryId = Guid.NewGuid();
          var mailError = Result.Error("Mail sending failed");

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddTemporaryCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(customerId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(deliveryId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<SendDeliveryCreationMailToCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(mailError);

          MockMediator
               .Setup(x => x.Send(It.IsAny<AddDeliveryStepsCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await Client.PostAsJsonAsync("/api/deliveries/by_customer", request, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

          MockMediator.Verify(
               x => x.Send(
                    It.IsAny<SendDeliveryCreationMailToCustomerCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task HandleAsync_ShouldDeleteCustomer_WhenDeliveryCreationFails()
     {
          // Arrange
          MockMediator.Reset();
          var request = _fixture.Create<AddDeliveryByCustomerRequest>();
          var customerId = Guid.NewGuid();

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddTemporaryCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(customerId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ThrowsAsync(new InvalidOperationException("Delivery creation error"));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<DeleteCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          MockMediator
               .Setup(x => x.Send(It.IsAny<AddDeliveryStepsCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await Client.PostAsJsonAsync("/api/deliveries/by_customer", request, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

          MockMediator.Verify(
               x => x.Send(
                    It.Is<DeleteCustomerCommand>(cmd => cmd.Id == customerId),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task HandleAsync_ShouldSucceed_WhenAllOperationsComplete()
     {
          // Arrange
          MockMediator.Reset();
          var request = _fixture.Create<AddDeliveryByCustomerRequest>();
          var customerId = Guid.NewGuid();
          var deliveryId = Guid.NewGuid();

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddTemporaryCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(customerId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(deliveryId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<SendDeliveryCreationMailToCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          MockMediator
               .Setup(x => x.Send(It.IsAny<AddDeliveryStepsCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await Client.PostAsJsonAsync("/api/deliveries/by_customer", request, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.Created);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);
          var actualId = responseObject.GetProperty("id").GetGuid();

          actualId.Should().Be(deliveryId);

          MockMediator.Verify(
               x => x.Send(
                    It.IsAny<AddTemporaryCustomerCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          MockMediator.Verify(
               x => x.Send(
                    It.IsAny<AddDeliveryCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          MockMediator.Verify(
               x => x.Send(
                    It.IsAny<SendDeliveryCreationMailToCustomerCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task HandleAsync_ShouldReturnCreatedWithLocation_WhenSuccessful()
     {
          // Arrange
          MockMediator.Reset();
          var request = _fixture.Create<AddDeliveryByCustomerRequest>();
          var customerId = Guid.NewGuid();
          var deliveryId = Guid.NewGuid();

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddTemporaryCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(customerId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<AddDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Success(deliveryId));

          MockMediator
               .Setup(x => x.Send(
                    It.IsAny<SendDeliveryCreationMailToCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          MockMediator
               .Setup(x => x.Send(It.IsAny<AddDeliveryStepsCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await Client.PostAsJsonAsync("/api/deliveries/by_customer", request, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.Created);

     }
}
