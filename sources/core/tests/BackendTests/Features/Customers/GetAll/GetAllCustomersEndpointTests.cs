using AutoFixture;
using AwesomeAssertions;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers;
using Ggio.BikeSherpa.Backend.Features.Customers.GetAll;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Customers.GetAll;

public class GetAllCustomersEndpointTests
{
     private readonly Mock<IMediator> _mockMediator = new();
     private readonly Mock<ICustomerLinks> _mockLinksService = new();
     private readonly Fixture _fixture = new();

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenCustomersExist()
     {
          // Arrange
          var expectedCustomers = _fixture.Create<List<CustomerCrud>>();
          var sut = CreateSut(expectedCustomers);

          // Act
          await sut.HandleAsync(CancellationToken.None);

          // Assert
          VerifyMediatorCalledOnce();
          sut.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
          sut.Response.Should().HaveCount(expectedCustomers.Count);
          sut.Response.Select(x => x.Data).Should().BeEquivalentTo(expectedCustomers);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenNoCustomersExist()
     {
          // Arrange
          var sut = CreateSut(new List<CustomerCrud>());

          // Act
          await sut.HandleAsync(CancellationToken.None);

          // Assert
          VerifyMediatorCalledOnce();
          sut.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
          sut.Response.Should().BeEmpty();
     }

     private GetAllCustomersEndpoint CreateSut(List<CustomerCrud> returnCustomers)
     {
          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetAllCustomersQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(returnCustomers);

          Factory.RegisterTestServices(s =>
          {
               s.AddSingleton(_mockMediator.Object);
               s.AddSingleton(_mockLinksService.Object);
          });

          var endpoint = Factory.Create<GetAllCustomersEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "GET";
                    ctx.Request.Path = "/api/customers";
               },
               _mockMediator.Object,
               _mockLinksService.Object
          );

          return endpoint;
     }

     private void VerifyMediatorCalledOnce()
     {
          _mockMediator.Verify(
               m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()),
               Times.Once
          );
     }
}
