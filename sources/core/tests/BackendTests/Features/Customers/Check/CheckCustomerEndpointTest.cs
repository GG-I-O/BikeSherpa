using System.Net;
using System.Text.Json;
using Ardalis.Specification;
using AwesomeAssertions;
using AutoFixture;
using BackendTests.Services;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Customers.Check;
using Ggio.DddCore;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Customers.Check;

[UsedImplicitly]
public class CheckCustomerWebApplicationFactory(
     Mock<IReadRepository<Customer>> repositoryMock,
     IValidator<CheckCustomerQuery> validatorMock)
     : TestWebApplicationFactory
{
     override protected void ConfigureWebHost(IWebHostBuilder builder)
     {
          base.ConfigureWebHost(builder);

          builder.ConfigureServices(services =>
          {
               services.AddSingleton(repositoryMock.Object);
               services.AddSingleton(validatorMock);
          });
     }
}

[TestSubject(typeof(CheckCustomerEndpoint))]
public class CheckCustomerEndpointTest
{
     private readonly static Fixture Fixture = new();

     private readonly static JsonSerializerOptions JsonOptions = new()
     {
          PropertyNameCaseInsensitive = true,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     private static HttpClient MakeSut(
          out Mock<IReadRepository<Customer>> repositoryMock)
     {
          repositoryMock = new Mock<IReadRepository<Customer>>();
          var validatorMock = new CheckCustomerQueryValidator();
        

          var factory = new CheckCustomerWebApplicationFactory(repositoryMock, validatorMock);
          return factory.CreateClient();
     }

     private static Customer MakeCustomer(string name, string code, string email, DeliveryType? deliveryType = null)
          => new()
          {
               Name = name,
               Code = code,
               Email = email,
               PhoneNumber = "0600000000",
               Address = Fixture.Create<Address>(),
               DefaultDeliveryType = deliveryType
          };

     [Fact]
     public async Task HandleAsync_CustomerFound_ReturnsOk()
     {
          // Arrange
          var ct = TestContext.Current.CancellationToken;
          var client = MakeSut(out var repositoryMock);

          var customer = MakeCustomer("Acme Corp", "CUST01", "test@example.com", DeliveryType.Simple);

          repositoryMock
               .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);

          // Act
          var response = await client.PostAsync(
               "/api/customers/check?Code=CUST01&Email=test%40example.com",
               null,
               ct);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);
     }

     [Fact]
     public async Task HandleAsync_CustomerFound_ReturnsCustomerNameAndDeliveryType()
     {
          // Arrange
          var ct = TestContext.Current.CancellationToken;
          var client = MakeSut(out var repositoryMock);

          var customer = MakeCustomer("Acme Corp", "CUST01", "test@example.com", DeliveryType.Simple);

          repositoryMock
               .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);

          // Act
          var response = await client.PostAsync(
               "/api/customers/check?Code=CUST01&Email=test%40example.com",
               null,
               ct);

          // Assert
          var body = await response.Content.ReadAsStringAsync(ct);
          var result = JsonSerializer.Deserialize<CheckCustomerResponse>(body, JsonOptions);

          result.Should().NotBeNull();
          result.CustomerName.Should().Be(customer.Name);
          result.DefaultDeliveryType.Should().Be(customer.DefaultDeliveryType);
     }

     [Fact]
     public async Task HandleAsync_CustomerFound_CallsRepositoryOnce()
     {
          // Arrange
          var ct = TestContext.Current.CancellationToken;
          var client = MakeSut(out var repositoryMock);

          var customer = MakeCustomer("Acme Corp", "CUST01", "test@example.com");

          repositoryMock
               .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);

          // Act
          await client.PostAsync(
               "/api/customers/check?Code=CUST01&Email=test%40example.com",
               null,
               ct);

          // Assert
          repositoryMock.Verify(
               r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task HandleAsync_CustomerNotFound_ReturnsNotFound()
     {
          // Arrange
          var ct = TestContext.Current.CancellationToken;
          var client = MakeSut(out var repositoryMock);

          repositoryMock
               .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((Customer?)null);

          // Act
          var response = await client.PostAsync(
               "/api/customers/check?Code=UNKNOWN&Email=nobody%40example.com",
               null,
               ct);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);
     }

     [Fact]
     public async Task HandleAsync_CustomerNotFound_CallsRepositoryOnce()
     {
          // Arrange
          var ct = TestContext.Current.CancellationToken;
          var client = MakeSut(out var repositoryMock);

          repositoryMock
               .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((Customer?)null);

          // Act
          await client.PostAsync(
               "/api/customers/check?Code=UNKNOWN&Email=nobody%40example.com",
               null,
               ct);

          // Assert
          repositoryMock.Verify(
               r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()),
               Times.Once);
     }

    

     [Fact]
     public async Task HandleAsync_CustomerFoundWithNoDeliveryType_ReturnsNullDeliveryType()
     {
          // Arrange
          var ct = TestContext.Current.CancellationToken;
          var client = MakeSut(out var repositoryMock);

          var customer = MakeCustomer("Acme Corp", "CUST01", "test@example.com", deliveryType: null);

          repositoryMock
               .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(customer);

          // Act
          var response = await client.PostAsync(
               "/api/customers/check?Code=CUST01&Email=test%40example.com",
               null,
               ct);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          var body = await response.Content.ReadAsStringAsync(ct);
          var result = JsonSerializer.Deserialize<CheckCustomerResponse>(body, JsonOptions);

          result!.DefaultDeliveryType.Should().BeNull();
     }
}
