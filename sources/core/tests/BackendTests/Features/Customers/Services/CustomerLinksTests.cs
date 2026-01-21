using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using AutoFixture;
using Ggio.BikeSherpa.Backend.Model;

namespace BackendTests.Features.Customers.Services;

public class CustomerLinksTests
{
     private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
     private readonly Mock<IHateoasService> _mockHateoasService = new();
     private readonly Fixture _fixture = new();

     private CustomerLinks CreateSut()
     {
          return new CustomerLinks(_mockHttpContextAccessor.Object, _mockHateoasService.Object);
     }

     private void setupHttpContextScope(string scope)
     {
          var claims = new List<Claim>
          {
               new Claim("scope", scope)
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
     [InlineData("read:customers")]
     [InlineData("write:customers")]
     [InlineData("write:customers read:customers")]
     public void GenerateLinks_ShouldCallHateoas_DependingOnScope(string scope)
     {
          // Arrange
          setupHttpContextScope(scope);
          var sut = CreateSut();
          var id = Guid.NewGuid();

          var canRead = scope.Contains("read:customers");
          var canWrite = scope.Contains("write:customers");

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
