using System.Security.Claims;
using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Services;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BackendTests.Features.Deliveries.Services;

public class DeliveryLinksTests
{
     private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
     private readonly Mock<IHateoasService> _mockHateoasService = new();
     private readonly Fixture _fixture = new();

     private DeliveryLinks CreateSut()
     {
          return new DeliveryLinks(_mockHttpContextAccessor.Object, _mockHateoasService.Object);
     }

     private void SetupHttpContextScope(string scope)
     {
          var claims = new List<Claim>
          {
               new("scope", scope)
          };

          var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
          _mockHttpContextAccessor.Setup(x => x.HttpContext!.User).Returns(principal);
     }

     [Fact]
     public void GenerateLinks_ShouldReturnEmptyList_WhenContextIsNull()
     {
          // Arrange
          _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(null as HttpContext);
          var sut = CreateSut();

          // Act
          var links = sut.GenerateLinks(Guid.Empty);

          // Assert
          links.Should().BeEmpty();
     }

     [Theory]
     [InlineData("")]
     [InlineData("badScope")]
     [InlineData("read:deliveries")]
     [InlineData("write:deliveries")]
     [InlineData("write:deliveries read:deliveries")]
     public void GenerateLinks_ShouldCallHateoas_DependingOnScope(string scope)
     {
          // Arrange
          SetupHttpContextScope(scope);
          var sut = CreateSut();
          var id = Guid.NewGuid();

          var canRead = scope.Contains("read:deliveries");
          var canWrite = scope.Contains("write:deliveries");

          List<Link> mockList = (canRead || canWrite) ? _fixture.CreateMany<Link>().ToList() : [];
          _mockHateoasService
               .Setup(x => x.GenerateLinks(
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<object>())
               )
               .Returns(mockList);

          // Act
          var links = sut.GenerateLinks(id);

          // Assert
          if (canRead || canWrite)
          {
               links.Should().NotBeEmpty();
               links.Should().BeEquivalentTo(mockList);

               _mockHateoasService.Verify(x => x.GenerateLinks(
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<object>()
               ), Times.Once);
          }
          else
          {
               links.Should().BeEmpty();
               _mockHateoasService.Verify(x => x.GenerateLinks(
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<object>()
               ), Times.Never);
          }
     }
}
