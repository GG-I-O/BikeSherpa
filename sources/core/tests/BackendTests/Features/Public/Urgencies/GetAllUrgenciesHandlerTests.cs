using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Features.Public.Urgencies.GetAll;
using Ggio.BikeSherpa.Backend.Features.Public.Urgencies.Model;
using Moq;

namespace BackendTests.Features.Public.Urgencies;

public class GetAllUrgenciesHandlerTests
{
     private readonly Mock<IUrgencyRepository> _mockRepository = new();
     private readonly IFixture _fixture = new Fixture();
     private readonly Urgency _mockUrgencyA;
     private readonly Urgency _mockUrgencyB;

     public GetAllUrgenciesHandlerTests()
     {
          _mockUrgencyA = _fixture.Create<Urgency>();
          _mockUrgencyB = _fixture.Create<Urgency>();
     }

     [Fact]
     public async Task Handle_ShouldReturnAllUrgencies_WhenUrgenciesExist()
     {
          // Arrange
          var urgencies = new List<Urgency>
          {
               _mockUrgencyA,
               _mockUrgencyB
          };

          var sut = CreateSut(urgencies);
          var query = new GetAllUrgenciesQuery();
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          result.Should().NotBeNull();
          result.Count.Should().Be(2);
          result[0].Label.Should().Be(_mockUrgencyA.Name);
          result[1].Value.Should().Be(_mockUrgencyB.Name);
          VerifyRepositoryCalledOnce();
     }
     
     [Fact]
     public async Task Handle_ShouldReturnEmptyList_WhenNoUrgencyExist()
     {
          // Arrange
          var sut = CreateSut([]);
          var query = new GetAllUrgenciesQuery();

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Should().NotBeNull();
          result.Should().BeEmpty();
          VerifyRepositoryCalledOnce();
     }

     private GetAllUrgenciesHandler CreateSut(List<Urgency> returnCustomers)
     {
          _mockRepository
               .Setup(repo =>
                    repo.GetAll()
               )
               .Returns(returnCustomers);

          return new GetAllUrgenciesHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(
               repo => repo.GetAll(),
               Times.Once
          );
     }
}
