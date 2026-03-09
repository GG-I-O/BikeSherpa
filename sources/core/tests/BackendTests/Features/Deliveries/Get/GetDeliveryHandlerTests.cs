using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Get;

public class GetDeliveryHandlerTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Delivery _mockDelivery;

     public GetDeliveryHandlerTests()
     {
          _mockDelivery = _fixture.Create<Delivery>();
     }

     [Fact]
     public async Task Handle_ShouldReturnOneDelivery_WhenDeliveryExists()
     {
          // Arrange
          var guid = Guid.NewGuid();
          _mockDelivery.Id = guid;
          _mockDelivery.Code = "AAA";
          _mockDelivery.PricingStrategy = PricingStrategy.CustomStrategy;
          _mockDelivery.PackingSize = "Xl";
          var sut = CreateSut(_mockDelivery);
          var query = new GetDeliveryQuery(guid);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().NotBeNull();
          result.Id.Should().Be(guid);
          result.Code.Should().Be("AAA");
          result.PricingStrategy.Should().Be(PricingStrategy.CustomStrategy);
          result.PackingSize.Should().Be("Xl");
          VerifyRepositoryCalledOnce();
     }

     [Theory]
     [InlineData(true)]
     [InlineData(false)]
     public async Task Handle_ShouldReturnNull_WhenDeliveryDoesNotExist(bool emptyRepository)
     {
          // Arrange
          var guidA = Guid.NewGuid();
          var guidB = Guid.NewGuid();
          _mockDelivery.Id = guidA;
          var sut = CreateSut(emptyRepository ? null : _mockDelivery);
          var query = new GetDeliveryQuery(guidB);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().BeNull();
          VerifyRepositoryCalledOnce();
     }

     private GetDeliveryHandler CreateSut(Delivery? existingDelivery)
     {
          _mockRepository
               .Setup(repo => repo.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification && existingDelivery != null && s.IsSatisfiedBy(existingDelivery)),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(existingDelivery);
          return new GetDeliveryHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(repo => repo.FirstOrDefaultAsync(
               It.IsAny<ISpecification<Delivery>>(),
               It.IsAny<CancellationToken>()),
               Times.Once
          );
     }
}
