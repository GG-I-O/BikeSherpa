using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;
using Ggio.DddCore;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Moq;

namespace BackendTests.Features.Deliveries.Patch;

public class PatchDeliveryStepTimeHandlerTest
{
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Guid _deliveryId;
     private readonly Guid _stepId;

     public PatchDeliveryStepTimeHandlerTest()
     {
          _deliveryId = Guid.NewGuid();
          _stepId = Guid.NewGuid();

          var mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Id, _deliveryId)
               .With(d => d.Steps, [])
               .Create();

          var mockStep = _fixture.Build<DeliveryStep>()
               .With(s => s.Id, _stepId)
               .With(s => s.ParentDelivery, mockDelivery)
               .Create();

          mockDelivery.Steps.Add(mockStep);

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(mockDelivery);
     }

     private PatchDeliveryStepTimeHandler CreateSut()
     {
          return new PatchDeliveryStepTimeHandler(
               _mockDeliveryRepository.Object,
               _mockTransaction.Object
          );
     }

     private static PatchDeliveryRequest CreateRequest(Guid deliveryId, Guid stepId, string patchValue)
     {
          var patchDocument = new JsonPatchDocument<DeliveryStep>();
          patchDocument.Operations.Add(new Operation<DeliveryStep>()
          {
               op = "replace",
               path = "/estimatedDeliveryDate",
               value = patchValue
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
          var request = CreateRequest(_deliveryId, _stepId, DateTimeOffset.UtcNow.ToString("O"));
          var command = new PatchDeliveryStepTimeCommand(request);

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
          var request = CreateRequest(_deliveryId, _stepId, DateTimeOffset.UtcNow.ToString("O"));
          var command = new PatchDeliveryStepTimeCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnError_WhenPatchValueIsNotADateTime()
     {
          // Arrange
          var sut = CreateSut();
          var request = CreateRequest(_deliveryId, _stepId, "not-a-date");
          var command = new PatchDeliveryStepTimeCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsError().Should().BeTrue();
          result.Errors.Should().Contain("estimatedDeliveryDate must be a valid ISO-8601 datetime.");
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }
}