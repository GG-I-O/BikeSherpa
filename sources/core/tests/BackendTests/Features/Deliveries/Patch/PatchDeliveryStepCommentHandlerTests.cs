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

public class PatchDeliveryStepCommentHandlerTests
{
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Guid _deliveryId;
     private readonly Guid _stepId;
     private readonly Delivery _mockDelivery;
     private readonly DeliveryStep _mockStep;

     public PatchDeliveryStepCommentHandlerTests()
     {
          _deliveryId = Guid.NewGuid();
          _stepId = Guid.NewGuid();

          _mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Id, _deliveryId)
               .With(d => d.Steps, [])
               .Create();

          _mockStep = _fixture.Build<DeliveryStep>()
               .With(s => s.Id, _stepId)
               .With(s => s.ParentDelivery, _mockDelivery)
               .Create();

          _mockDelivery.Steps.Add(_mockStep);

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_mockDelivery);
     }

     private PatchDeliveryStepCommentHandler CreateSut()
     {
          return new PatchDeliveryStepCommentHandler(
               _mockDeliveryRepository.Object,
               _mockTransaction.Object
          );
     }

     private static PatchDeliveryRequest CreateRequest(Guid deliveryId, Guid stepId, string? patchValue)
     {
          var patchDocument = new JsonPatchDocument<DeliveryStep>();
          patchDocument.Operations.Add(new Operation<DeliveryStep>()
          {
               op = "replace",
               path = "/comment",
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
          const string comment = "Leave the package at the reception desk.";
          var request = CreateRequest(_deliveryId, _stepId, comment);
          var command = new PatchDeliveryStepCommentCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          _mockStep.Comment.Should().Be(comment);
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldReturnSuccess_AndSetCommentToNull_WhenPatchValueIsNull()
     {
          // Arrange
          var sut = CreateSut();
          _mockStep.Comment = "Existing comment";
          var request = CreateRequest(_deliveryId, _stepId, null);
          var command = new PatchDeliveryStepCommentCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          _mockStep.Comment.Should().BeNull();
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
          var request = CreateRequest(_deliveryId, _stepId, "Comment");
          var command = new PatchDeliveryStepCommentCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenStepDoesNotExist()
     {
          // Arrange
          var sut = CreateSut();
          var request = CreateRequest(_deliveryId, Guid.NewGuid(), "Comment");
          var command = new PatchDeliveryStepCommentCommand(request);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }
}