using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;
using Ggio.DddCore;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Moq;

namespace BackendTests.Features.Deliveries.Patch;

public class PatchDeliveryStepOrderHandlerTests
{
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IItinerarySpi> _mockItineraryService = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Guid _deliveryId;
     private readonly Guid _stepId;

     public PatchDeliveryStepOrderHandlerTests()
     {
          _deliveryId = Guid.NewGuid();
          _stepId = Guid.NewGuid();

          var mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Id, _deliveryId)
               .With(d => d.Steps, [])
               .Create();

          var mockSteps = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, mockDelivery)
               .CreateMany(5)
               .ToList();
          mockDelivery.Steps.AddRange(mockSteps);

          _stepId = mockDelivery.Steps.First().Id;

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(mockDelivery);
          
          _mockItineraryService
               .Setup(x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ItineraryResult(12.3, 45));
     }

     private PatchDeliveryStepOrderHandler CreateSut()
     {
          return new PatchDeliveryStepOrderHandler(
               _mockDeliveryRepository.Object,
               _mockTransaction.Object,
               _mockItineraryService.Object
          );
     }

     private static PatchDeliveryRequest CreateRequest(Guid deliveryId, Guid stepId, int newOrder)
     {
          var patchDocument = new JsonPatchDocument<DeliveryStep>();
          patchDocument.Operations.Add(new Operation<DeliveryStep>()
          {
               op = "replace",
               path = "/order",
               value = newOrder.ToString()
          });

          return new PatchDeliveryRequest
          {
               DeliveryId = deliveryId,
               StepId = stepId,
               Patches = patchDocument
          };
     }

     [Fact]
     public async Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
     {
          // Arrange
          var sut = CreateSut();
          var request = CreateRequest(_deliveryId, _stepId, 4);
          var command = new PatchDeliveryStepOrderCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenDeliveryDoesNotExist()
     {
          // Arrange
          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Delivery);

          var sut = CreateSut();
          var request = CreateRequest(_deliveryId, _stepId, 4);
          var command = new PatchDeliveryStepOrderCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnError_WhenPatchValueIsNotAnInteger()
     {
          // Arrange
          var sut = CreateSut();
          var request = CreateRequest(_deliveryId, _stepId, 0);
          request.Patches.Operations[0].value = "not-an-int";
          var command = new PatchDeliveryStepOrderCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsError().Should().BeTrue();
          result.Errors.Should().Contain("Invalid step order value provided.");
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }
}