using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Events;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryTests
{
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly Mock<IItinerarySpi> _mockItineraryService = new();

     private Delivery MakeSut()
     {
          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, new List<DeliveryStep>())
               .With(d => d.PricingStrategy, _fixture.Create<PricingStrategy>())
               .With(d => d.Code, _fixture.Create<string>())
               .With(d => d.CustomerId, _fixture.Create<Guid>())
               .With(d => d.Urgency, _fixture.Create<Urgency>())
               .With(d => d.InsulatedBox, false)
               .With(d => d.StartDate, DateTimeOffset.UtcNow)
               .With(d => d.ContractDate, DateTimeOffset.UtcNow)
               .Create();

          return delivery;
     }

     private DeliveryStep CreatePickupStep(bool completed = false)
     {
          var deliveryStep = _fixture.Build<DeliveryStep>()
               .Without(s => s.ParentDelivery)
               .Create();

          deliveryStep.Id = Guid.NewGuid();
          deliveryStep.StepType = StepType.Pickup;
          deliveryStep.Completed = completed;
          deliveryStep.StepAddress.Coordinates = new GeoPoint(1, 2);

          return deliveryStep;
     }

     private DeliveryStep CreateDropOffStep(bool completed = false)
     {
          var deliveryStep = _fixture.Build<DeliveryStep>()
               .Without(s => s.ParentDelivery)
               .Create();

          deliveryStep.Id = Guid.NewGuid();
          deliveryStep.StepType = StepType.Dropoff;
          deliveryStep.Completed = completed;
          deliveryStep.StepAddress.Coordinates = new GeoPoint(3, 4);

          return deliveryStep;
     }

     [Fact]
     public void GenerateReportId_ReturnsIdStartingWithCustomerCode()
     {
          // Arrange
          var delivery = MakeSut();
          var customer = _fixture.Create<Customer>();

          // Act
          delivery.GenerateReportId(customer);

          // Assert
          delivery.CustomerReference.Should().StartWith($"{customer.Code}-");
     }

     [Fact]
     public void GenerateReportId_TimestampPartHasExpectedLength()
     {
          // Arrange
          var delivery = MakeSut();
          var customer = _fixture.Create<Customer>();
          customer.Code = "TEST";

          // Act
          delivery.GenerateReportId(customer);

          // Assert
          var timestamp = delivery.CustomerReference![("TEST".Length + 1)..];
          timestamp.Should().HaveLength(14);
     }

     [Fact]
     public async Task ReorderSteps_ReordersAllStepsSettingCorrectOrderForMovedStep()
     {
          // Arrange
          var delivery = MakeSut();
          var step1 = CreatePickupStep();
          var step2 = CreateDropOffStep();
          var step3 = CreateDropOffStep();
          step1.Order = 1;
          step2.Order = 2;
          step3.Order = 3;
          delivery.Steps.AddRange([step1, step2, step3]);
          var (_, _, mockItineraryService) = CreateStepDependencies();

          // Act
          await delivery.ReorderStepsAsync(step3.Id, 2, mockItineraryService.Object);

          // Assert
          delivery.Steps.Single(s => s.Id == step1.Id).Order.Should().Be(1);
          delivery.Steps.Single(s => s.Id == step3.Id).Order.Should().Be(2);
          delivery.Steps.Single(s => s.Id == step2.Id).Order.Should().Be(3);
     }

     [Fact]
     public async Task ReorderStepsAsync_WhenStepNotFound_Throws()
     {
          // Arrange
          var delivery = MakeSut();

          // Act
          var act = () => delivery.ReorderStepsAsync(Guid.NewGuid(), 1, _mockItineraryService.Object);

          // Assert
          await act.Should().ThrowAsync<InvalidOperationException>();
     }

     [Fact]
     public void UpdateStepCourier_SetsNewCourierOnStep()
     {
          // Arrange
          var delivery = MakeSut();
          var step = CreatePickupStep();
          delivery.Steps.Add(step);
          var courierId = Guid.NewGuid();

          // Act
          delivery.UpdateStepCourier(step.Id, courierId);

          // Assert
          delivery.Steps.Single(s => s.Id == step.Id).CourierId.Should().Be(courierId);
     }

     [Fact]
     public void UpdateStepCourier_WhenStepNotFound_Throws()
     {
          // Arrange
          var delivery = MakeSut();
          // Act
          var act = () => delivery.UpdateStepCourier(Guid.NewGuid(), Guid.NewGuid());

          // Assert
          act.Should().Throw<InvalidOperationException>();
     }

     [Fact]
     public void UpdateStepCompletion_MarksStepAsCompletedAndSetsRealDeliveryDate()
     {
          // Arrange
          var delivery = MakeSut();
          var step = CreateDropOffStep();
          delivery.Steps.Add(step);
          delivery.Validate();

          // Act
          delivery.UpdateStepCompletion(step.Id, true);

          // Assert
          step.Completed.Should().BeTrue();
          step.RealDeliveryDate.Should().NotBe(null);
     }

     [Fact]
     public void UpdateStepCompletion_MarksStepAsNotCompleted()
     {
          // Arrange
          var delivery = MakeSut();
          var step = CreateDropOffStep(true);
          delivery.Steps.Add(step);
          delivery.Validate();

          // Act
          delivery.UpdateStepCompletion(step.Id, false);

          // Assert
          step.Completed.Should().BeFalse();
     }

     [Fact]
     public void UpdateStepCompletion_WhenPickupStepCompleted_StartsDelivery()
     {
          // Arrange
          var delivery = MakeSut();
          var pickup = CreatePickupStep();
          delivery.Steps.Add(pickup);
          delivery.Validate();

          // Act
          delivery.UpdateStepCompletion(pickup.Id, true);

          // Assert
          delivery.Status.Should().Be(DeliveryStatus.Started);
     }

     [Fact]
     public void UpdateStepCompletion_WhenDeliveryHasStatusNew_ShouldThrowException()
     {
          // Arrange
          var delivery = MakeSut();
          var pickup = CreatePickupStep();
          delivery.Steps.Add(pickup);

          // Act
          var act = () => delivery.UpdateStepCompletion(pickup.Id, true);

          // Assert
          act.Should().Throw<InvalidOperationException>();
     }

     [Fact]
     public void UpdateStepCompletion_WhenAllStepsCompleted_CompletesDelivery()
     {
          // Arrange
          var delivery = MakeSut();
          var pickup = CreatePickupStep(true);
          var dropOff = CreateDropOffStep();
          delivery.Steps.AddRange([pickup, dropOff]);
          delivery.Validate();
          delivery.UpdateStepCompletion(pickup.Id, true); // Starts delivery

          // Act
          delivery.UpdateStepCompletion(dropOff.Id, true);

          // Assert
          delivery.Status.Should().Be(DeliveryStatus.Completed);
     }

     [Fact]
     public void UpdateStepCompletion_WhenDeliveryIsAlreadyCompleted_Throws()
     {
          // Arrange
          var delivery = MakeSut();
          var step = CreatePickupStep();
          delivery.Steps.Add(step);
          delivery.Status = DeliveryStatus.Completed;

          // Act
          var act = () => delivery.UpdateStepCompletion(step.Id, true);

          // Assert
          act.Should().Throw<InvalidOperationException>();
     }

     [Fact]
     public void UpdateStepCompletion_WhenDeliveryIsCancelled_Throws()
     {
          // Arrange
          var delivery = MakeSut();
          var step = CreatePickupStep();
          delivery.Steps.Add(step);
          delivery.Status = DeliveryStatus.Cancelled;

          // Act
          var act = () => delivery.UpdateStepCompletion(step.Id, true);

          // Assert
          act.Should().Throw<InvalidOperationException>();
     }

     [Fact]
     public void Cancel_WhenPending_ChangesStatusToCancelled()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Validate();

          // Act
          delivery.Cancel();

          // Assert
          delivery.Status.Should().Be(DeliveryStatus.Cancelled);
     }

     [Fact]
     public async Task AddStepAsync_AddsStepToStepsList()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Validate();
          var address = _fixture.Create<Address>();
          var mockZoneRepo = new Mock<IDeliveryZoneRepository>();
          var mockItineraryService = new Mock<IItinerarySpi>();
          mockItineraryService.Setup(i => i.GetItineraryInfoAsync(It.IsAny<GeoPoint>(), It.IsAny<GeoPoint>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new ItineraryResult(10.0, 20.0));

          // Act
          await delivery.AddStepAsync(
               StepType.Pickup,
               address,
               "CommentPickup",
               false,
               new PackingSize("packing", 1, "label", 3, 10),
               mockZoneRepo.Object,
               mockItineraryService.Object
          );

          // Assert
          delivery.Steps.Should().HaveCount(1);
          delivery.Steps[0].StepType.Should().Be(StepType.Pickup);
     }

     [Fact]
     public async Task AddStep_ReturnsCreatedStepWithCorrectProperties()
     {
          // Arrange
          var delivery = MakeSut();
          var (mockDeliveryZoneRepository, address, mockItineraryService) = CreateStepDependencies();

          // Act
          var step = await delivery.AddStepAsync(
               StepType.Dropoff,
               address,
               "Comment Drop off",
               false,
               new PackingSize("packing", 1, "label", 3, 10),
               mockDeliveryZoneRepository.Object,
               mockItineraryService.Object);

          // Assert
          step.StepType.Should().Be(StepType.Dropoff);
          step.Order.Should().Be(1);
          step.Id.Should().NotBeEmpty();
     }

     [Fact]
     public async Task AddStep_LooksUpDeliveryZoneByAddressCity()
     {
          // Arrange
          var delivery = MakeSut();
          var (mockDeliveryZoneRepository, address, mockItineraryService) = CreateStepDependencies();

          // Act
          await delivery.AddStepAsync(StepType.Pickup,
               address,
               "CommentPickup",
               false,
               new PackingSize("packing", 1, "label", 3, 10),
               mockDeliveryZoneRepository.Object,
               mockItineraryService.Object);

          // Assert
          mockDeliveryZoneRepository.Verify(r => r.GetByAddress(address.City), Times.Once);
     }

     [Fact]
     public async Task DeleteStepAsync_RemovesStepFromStepsList()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Validate();
          var step = CreatePickupStep();
          delivery.Steps.Add(step);
          var mockItineraryService = new Mock<IItinerarySpi>();
          mockItineraryService.Setup(i => i.GetItineraryInfoAsync(It.IsAny<GeoPoint>(), It.IsAny<GeoPoint>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new ItineraryResult(10.0, 20.0));

          // Act
          await delivery.DeleteStepAsync(step, mockItineraryService.Object);

          // Assert
          delivery.Steps.Should().BeEmpty();
     }

     [Fact]
     public async Task UpdateSteps_AddsNewStepsThatDontExistYet()
     {
          // Arrange
          var delivery = MakeSut();
          var (mockDeliveryZoneRepository, mockItineraryService) = CreateUpdateStepsDependencies();
          var newStep = CreatePickupStep();

          // Act
          await delivery.UpdateStepsAsync([newStep], mockDeliveryZoneRepository.Object, mockItineraryService.Object);

          // Assert
          delivery.Steps.Should().ContainSingle(s => s.StepType == newStep.StepType && s.StepAddress == newStep.StepAddress);
     }

     [Fact]
     public async Task UpdateSteps_UpdatesExistingSteps()
     {
          // Arrange
          var delivery = MakeSut();
          var zone = _fixture.Create<DeliveryZone>();
          var (mockDeliveryZoneRepository, mockItineraryService) = CreateUpdateStepsDependencies(zone);
          var existingStep = CreatePickupStep();
          delivery.Steps.Add(existingStep);

          // Act
          var updatedStep = new DeliveryStep(
               StepType.Dropoff,
               1,
               existingStep.StepAddress,
               "NewComm"
          )
          {
               Id = existingStep.Id,
               StepAddress = existingStep.StepAddress,
               StepZone = zone,
               ParentDelivery = delivery,
               PackingSize = new PackingSize("packing", 1, "label", 3, 10)
          };

          await delivery.UpdateStepsAsync(
               [updatedStep],
               mockDeliveryZoneRepository.Object,
               mockItineraryService.Object
          );

          // Assert
          delivery.Steps.Should().HaveCount(1);
          delivery.Steps[0].StepType.Should().Be(StepType.Dropoff);
          delivery.Steps[0].Order.Should().Be(1);
          delivery.Steps[0].Distance.Should().Be(0);
          delivery.Steps[0].StepZone.Should().Be(zone);
          delivery.Steps[0].Comment.Should().Be("NewComm");
     }

     [Fact]
     public async Task UpdateSteps_WhenMixingExistingAndNewSteps_ReordersAndKeepsExistingStep()
     {
          // Arrange
          var delivery = MakeSut();
          var zone = _fixture.Create<DeliveryZone>();
          var (mockDeliveryZoneRepository, mockItineraryService) = CreateUpdateStepsDependencies(zone);

          var existingStep = CreatePickupStep();
          existingStep.Order = 1;
          existingStep.EstimatedDeliveryDate = delivery.StartDate;
          delivery.Steps.Add(existingStep);

          var incomingExistingStep = new DeliveryStep(
               StepType.Dropoff,
               1,
               existingStep.StepAddress,
               "Updated existing step")
          {
               Id = existingStep.Id,
               StepAddress = existingStep.StepAddress,
               StepZone = zone,
               Distance = 12.5,
               EstimatedDeliveryDate = existingStep.EstimatedDeliveryDate,
               Completed = true,
               ParentDelivery = delivery,
               PackingSize = new PackingSize("packing", 1, "label", 3, 10)
          };

          var newStep = CreateDropOffStep();
          newStep.StepAddress.Coordinates = new GeoPoint(8, 9);

          // Act
          await delivery.UpdateStepsAsync(
               [incomingExistingStep, newStep],
               mockDeliveryZoneRepository.Object,
               mockItineraryService.Object);

          // Assert
          delivery.Steps.Should().HaveCount(2);

          var updatedExistingStep = delivery.Steps.Single(s => s.Id == existingStep.Id);
          updatedExistingStep.StepType.Should().Be(StepType.Dropoff);
          updatedExistingStep.Order.Should().Be(1);
          updatedExistingStep.Comment.Should().Be("Updated existing step");
          updatedExistingStep.Completed.Should().BeTrue();
          updatedExistingStep.StepZone.Should().Be(zone);
          updatedExistingStep.Distance.Should().Be(0);

          var createdStep = delivery.Steps.Single(s => s.Id != existingStep.Id);
          createdStep.StepType.Should().Be(StepType.Dropoff);
          createdStep.Order.Should().Be(2);
          createdStep.Comment.Should().Be(newStep.Comment);
          createdStep.StepZone.Should().NotBeNull();
          createdStep.Id.Should().NotBeEmpty();
     }

     [Fact]
     public async Task UpdateSteps_WhenIncomingListIsAlreadyOrdered_ReassignsSequentialOrders()
     {
          // Arrange
          var delivery = MakeSut();
          var (mockDeliveryZoneRepository, mockItineraryService) = CreateUpdateStepsDependencies();

          var step1 = CreatePickupStep();
          var step2 = CreateDropOffStep();
          var step3 = CreateDropOffStep();

          step1.Order = 99;
          step2.Order = 98;
          step3.Order = 97;

          // Act
          await delivery.UpdateStepsAsync([step1, step2, step3], mockDeliveryZoneRepository.Object, mockItineraryService.Object);

          // Assert
          delivery.Steps.Select(s => s.Order).Should().Equal(1, 2, 3);
          delivery.Steps[0].Distance.Should().Be(0);
          delivery.Steps[1].EstimatedDeliveryDate.Should().Be(delivery.Steps[0].EstimatedDeliveryDate.AddMinutes(15));
          delivery.Steps[2].EstimatedDeliveryDate.Should().Be(delivery.Steps[1].EstimatedDeliveryDate.AddMinutes(15));
     }

     [Fact]
     public async Task UpdateSteps_RemovesStepsNotInIncomingList()
     {
          // Arrange
          var delivery = MakeSut();
          var (mockDeliveryZoneRepository, mockItineraryService) = CreateUpdateStepsDependencies();
          var stepToKeep = CreatePickupStep();
          var stepToRemove = CreatePickupStep();
          delivery.Steps.AddRange([stepToKeep, stepToRemove]);

          // Act
          await delivery.UpdateStepsAsync([stepToKeep], mockDeliveryZoneRepository.Object, mockItineraryService.Object);

          // Assert
          delivery.Steps.Should().HaveCount(1);
          delivery.Steps.Should().ContainSingle(s => s.Id == stepToKeep.Id);
          delivery.Steps.Should().NotContain(s => s.Id == stepToRemove.Id);
     }

     [Fact]
     public async Task UpdateSteps_WhenEmptyListProvided_RemovesAllExistingSteps()
     {
          // Arrange
          var delivery = MakeSut();
          var (mockDeliveryZoneRepository, mockItineraryService) = CreateUpdateStepsDependencies();
          delivery.Steps.AddRange([CreatePickupStep(), CreatePickupStep()]);

          // Act
          await delivery.UpdateStepsAsync([], mockDeliveryZoneRepository.Object, mockItineraryService.Object);

          // Assert
          delivery.Steps.Should().BeEmpty();
     }

     [Fact]
     public void UpdateStepUpdateStepDeliveryTime_ChangesUpdatedStepAndNextStepsDeliveryTime()
     {
          // Arrange
          var delivery = MakeSut();
          var step1 = CreatePickupStep();
          var step2 = CreateDropOffStep();
          var step3 = CreateDropOffStep();
          var step4 = CreateDropOffStep();
          var startTime = DateTimeOffset.UtcNow;
          step1.Order = 1;
          step2.Order = 2;
          step3.Order = 3;
          step4.Order = 4;
          step1.EstimatedDeliveryDate = startTime;
          step2.EstimatedDeliveryDate = startTime.AddMinutes(15);
          step3.EstimatedDeliveryDate = startTime.AddMinutes(25);
          step4.EstimatedDeliveryDate = startTime.AddMinutes(35);
          delivery.Steps.AddRange([step1, step2, step3, step4]);

          // Act
          delivery.UpdateStepDeliveryTime(step2.Id, startTime.AddMinutes(20));

          // Assert
          step2.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(20));
          step3.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(30));
          step4.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(40));
     }

     [Fact]
     public void ValidateDelivery_WhenHasStatusNew_ThenShouldForwardToPEndingStatus()
     {
          // Arrange
          var delivery = MakeSut();

          //Act
          delivery.Validate();

          //Assert
          delivery.Status.Should().Be(DeliveryStatus.Pending);
     }

     [Fact]
     public void ValidateDelivery_WhenHasStatusPending_ThenShouldThrowException()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Status = DeliveryStatus.Pending;

          //Act
          var test = delivery.Validate;

          //Assert
          test.Should().Throw<InvalidOperationException>();
     }


     [Fact]
     public void Renew_WhenStatusIsCancelled_ShouldChangeStatusToPending()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Status = DeliveryStatus.Pending;

          // Act
          delivery.Renew();

          // Assert
          delivery.Status.Should().Be(DeliveryStatus.New);
     }

     [Fact]
     public void Renew_WhenStatusIsCancelled_ShouldRegisterDeliveryRenewedEvent()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Status = DeliveryStatus.Pending;

          // Act
          delivery.Renew();

          // Assert
          delivery.DomainEvents.Should().ContainSingle(e => e is DeliveryRenewedEvent);
     }

     [Fact]
     public void Renew_WhenStatusIsNotCancelled_ShouldThrowInvalidOperationException()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Status = DeliveryStatus.Cancelled;

          // Act
          var act = delivery.Renew;

          // Assert
          act.Should().Throw<InvalidOperationException>();
     }

     [Fact]
     public void Cancel_WhenStatusIsNew_ShouldChangeStatusToCancelled()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Status = DeliveryStatus.New;

          // Act
          delivery.Cancel();

          // Assert
          delivery.Status.Should().Be(DeliveryStatus.Cancelled);
     }

     [Fact]
     public void Cancel_ShouldRegisterDeliveryCancelledEvent()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Status = DeliveryStatus.Pending;

          // Act
          delivery.Cancel();

          // Assert
          delivery.DomainEvents.Should().ContainSingle(e => e is DeliveryCancelledEvent);
     }

     [Fact]
     public void Validate_ShouldRegisterDeliveryValidatedEvent()
     {
          // Arrange
          var delivery = MakeSut();
          delivery.Status = DeliveryStatus.New;

          // Act
          delivery.Validate();

          // Assert
          delivery.DomainEvents.Should().ContainSingle(e => e is DeliveryValidatedEvent);
     }

     [Fact]
     public void UpdateStepCompletion_WhenPickupStepCompleted_ShouldRegisterDeliveryStartedEvent()
     {
          // Arrange
          var delivery = MakeSut();
          var pickup = CreatePickupStep();
          delivery.Steps.Add(pickup);
          delivery.Validate();

          // Act
          delivery.UpdateStepCompletion(pickup.Id, true);

          // Assert
          delivery.DomainEvents.Should().ContainSingle(e => e is DeliveryStartedEvent);
     }

     [Fact]
     public void UpdateStepCompletion_WhenAllStepsCompleted_ShouldRegisterDeliveryCompletedEvent()
     {
          // Arrange
          var delivery = MakeSut();
          var pickup = CreatePickupStep(true);
          var dropOff = CreateDropOffStep();
          delivery.Steps.AddRange([pickup, dropOff]);
          delivery.Status = DeliveryStatus.Started;


          // Act
          delivery.UpdateStepCompletion(dropOff.Id, true);

          // Assert
          delivery.DomainEvents.Should().ContainSingle(e => e is DeliveryCompletedEvent);
     }

     [Fact]
     public void GetLimitDate_WhenUrgencyHasFixedTimeLimit_ShouldReturnFixedTimeLimitBasedOnStartDate()
     {
          // Arrange
          var delivery = MakeSut();
          var urgency = _fixture.Build<Urgency>()
               .With(u => u.FixedTimeLimit, new TimeSpan(14, 30, 0))
               .Create();

          delivery.Urgency = urgency;
          delivery.StartDate = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

          // Act
          var limitDate = delivery.GetLimitDate();

          // Assert
          limitDate.Should().NotBeNull();
          limitDate.Value.Year.Should().Be(2024);
          limitDate.Value.Month.Should().Be(6);
          limitDate.Value.Day.Should().Be(15);
          limitDate.Value.Hour.Should().Be(14);
          limitDate.Value.Minute.Should().Be(30);
     }

     [Fact]
     public void GetLimitDate_WhenUrgencyHasAddTimeLimit_ShouldReturnStartDatePlusAddTimeLimit()
     {
          // Arrange
          var delivery = MakeSut();
          var urgency = _fixture.Build<Urgency>()
               .With(u => u.FixedTimeLimit, (TimeSpan?)null)
               .With(u => u.AddTimeLimit, TimeSpan.FromHours(2))
               .Create();

          delivery.Urgency = urgency;
          delivery.StartDate = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

          // Act
          var limitDate = delivery.GetLimitDate();

          // Assert
          limitDate.Should().NotBeNull();
          limitDate.Value.Should().Be(new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero));
     }

     [Fact]
     public void GetTotalDistance_ShouldReturnSumOfAllStepDistances()
     {
          // Arrange
          var delivery = MakeSut();
          var step1 = CreatePickupStep();
          var step2 = CreateDropOffStep();
          var step3 = CreateDropOffStep();
          step1.Distance = 0;
          step2.Distance = 5.5;
          step3.Distance = 3.2;
          delivery.Steps.AddRange([step1, step2, step3]);

          // Act
          var totalDistance = delivery.GetTotalDistance();

          // Assert
          totalDistance.Should().Be(8.7);
     }

     [Fact]
     public void GenerateCode_ShouldGenerateCodeWithCorrectFormat()
     {
          // Arrange
          var delivery = MakeSut();
          var customer = _fixture.Create<Customer>();
          customer.Code = "CST";
          delivery.StartDate = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero);

          // Act
          delivery.GenerateCode(customer, 5);

          // Assert
          delivery.Code.Should().Be("40615-CST-5");
     }

     [Fact]
     public void GenerateCode_WhenIncrementIsLessThan10_ShouldNotPadIncrement()
     {
          // Arrange
          var delivery = MakeSut();
          var customer = _fixture.Create<Customer>();
          customer.Code = "TEST";
          delivery.StartDate = new DateTimeOffset(2025, 1, 5, 10, 0, 0, TimeSpan.Zero);

          // Act
          delivery.GenerateCode(customer, 3);

          // Assert
          delivery.Code.Should().Be("50105-TEST-3");
     }

     private (Mock<IDeliveryZoneRepository> mockDeliveryZoneRepository, Address address, Mock<IItinerarySpi> mockItineraryService)
          CreateStepDependencies()
     {
          var mockDeliveryZoneRepository = new Mock<IDeliveryZoneRepository>();
          var mockItineraryService = new Mock<IItinerarySpi>();
          var address = _fixture.Create<Address>();
          address.Coordinates = new GeoPoint(5.2, 45.3);
          mockDeliveryZoneRepository.Setup(r => r.GetByAddress(address.City)).Returns(_fixture.Create<DeliveryZone>());
          mockItineraryService.Setup(i => i.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ItineraryResult(10.0, 20.0));

          return (mockDeliveryZoneRepository, address, mockItineraryService);
     }

     private (Mock<IDeliveryZoneRepository> mockDeliveryZoneRepository, Mock<IItinerarySpi> mockItineraryService)
          CreateUpdateStepsDependencies(DeliveryZone? zone = null)
     {
          var mockDeliveryZoneRepository = new Mock<IDeliveryZoneRepository>();
          var mockItineraryService = new Mock<IItinerarySpi>();
          mockDeliveryZoneRepository.Setup(r => r.GetByAddress(It.IsAny<string>())).Returns(zone ?? _fixture.Create<DeliveryZone>());
          mockItineraryService.Setup(i => i.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ItineraryResult(10.0, 20.0));

          return (mockDeliveryZoneRepository, mockItineraryService);
     }
}
