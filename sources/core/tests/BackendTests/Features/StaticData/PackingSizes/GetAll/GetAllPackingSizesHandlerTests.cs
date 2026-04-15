using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Features.StaticData.PackingSizes.GetAll;
using Moq;

namespace BackendTests.Features.StaticData.PackingSizes.GetAll;

public class GetAllPackingSizesHandlerTests
{
     private readonly Mock<IPackingSizeRepository> _mockRepository = new();
     private readonly IFixture _fixture = new Fixture();
     private readonly PackingSize _mockPackingSizeA;
     private readonly PackingSize _mockPackingSizeB;

     public GetAllPackingSizesHandlerTests()
     {
          _mockPackingSizeA = _fixture.Create<PackingSize>();
          _mockPackingSizeB = _fixture.Create<PackingSize>();
     }
     
     [Fact]
     public async Task Handle_ShouldReturnAllUrgencies_WhenUrgenciesExist()
     {
          // Arrange
          var packingSizes = new List<PackingSize>
          {
               _mockPackingSizeA,
               _mockPackingSizeB
          };

          var sut = CreateSut(packingSizes);
          var query = new GetAllPackingSizesQuery();
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          result.Should().NotBeNull();
          result.Count.Should().Be(2);
          result[0].Label.Should().Be(_mockPackingSizeA.Name);
          result[1].Value.Should().Be(_mockPackingSizeB.Name);
          VerifyRepositoryCalledOnce();
     }
     
     [Fact]
     public async Task Handle_ShouldReturnEmptyList_WhenNoUrgencyExist()
     {
          // Arrange
          var sut = CreateSut([]);
          var query = new GetAllPackingSizesQuery();

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().NotBeNull();
          result.Should().BeEmpty();
          VerifyRepositoryCalledOnce();
     }

     private GetAllPackingSizesHandler CreateSut(List<PackingSize> returnPackingSizes)
     {
          _mockRepository
               .Setup(repo =>
                    repo.GetAll()
               )
               .Returns(returnPackingSizes);

          return new GetAllPackingSizesHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(
               repo => repo.GetAll(),
               Times.Once
          );
     }
}
