using AutoFixture;
using AwesomeAssertions;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Ggio.BikeSherpa.Backend.Infrastructure.Storage;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Backend.Infrastructure.Tests.Storage;

[TestSubject(typeof(StorageService))]
public class StorageServiceTest
{
     private static StorageService MakeSut(
          out Mock<BlobServiceClient> blobServiceClientMock,
          out Mock<BlobContainerClient> blobContainerClientMock,
          out Mock<BlobClient> blobClientMock,
          BlobStorageOptions? options = null)
     {
          var fixture = new Fixture();
          var resolvedOptions = options ?? new BlobStorageOptions
          {
               ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=dGVzdA==;EndpointSuffix=core.windows.net",
               ContainerName = fixture.Create<string>()
          };

          var optionsMock = new Mock<IOptions<BlobStorageOptions>>();
          optionsMock.Setup(o => o.Value).Returns(resolvedOptions);

          var loggerMock = new Mock<ILogger<StorageService>>();

          blobClientMock = new Mock<BlobClient>();
          blobClientMock.Setup(b => b.Uri).Returns(new Uri($"https://test.blob.core.windows.net/container/{fixture.Create<string>()}"));
          blobClientMock
               .Setup(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

          blobContainerClientMock = new Mock<BlobContainerClient>();
          blobContainerClientMock
               .Setup(c => c.GetBlobClient(It.IsAny<string>()))
               .Returns(blobClientMock.Object);

          blobContainerClientMock
               .Setup(c => c.CreateIfNotExistsAsync(It.IsAny<PublicAccessType>(), It.IsAny<IDictionary<string, string>>(), It.IsAny<BlobContainerEncryptionScopeOptions>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Mock.Of<Response<BlobContainerInfo>>());

          blobServiceClientMock = new Mock<BlobServiceClient>();
          blobServiceClientMock
               .Setup(s => s.GetBlobContainerClient(It.IsAny<string>()))
               .Returns(blobContainerClientMock.Object);

          return new StorageService(optionsMock.Object, loggerMock.Object, blobServiceClientMock.Object);
     }

     [Fact]
     public async Task StoreFileAsync_ValidInput_ReturnsNonEmptyUri()
     {
          // Arrange
          var fixture = new Fixture();
          var sut = MakeSut(
               out _,
               out _,
               out var blobClientMock);

          var content = new MemoryStream(fixture.CreateMany<byte>(64).ToArray());
          var fileName = fixture.Create<string>() + ".jpg";
          var contentType = "image/jpeg";

          // Act
          var result = await sut.StoreFileAsync(content, fileName, contentType, TestContext.Current.CancellationToken);

          // Assert
          result.Should().NotBeNullOrEmpty();
          result.Should().Be(blobClientMock.Object.Uri.ToString());
     }

     [Fact]
     public async Task StoreFileAsync_ValidInput_CallsUploadOnBlobClient()
     {
          // Arrange
          var fixture = new Fixture();
          var sut = MakeSut(
               out _,
               out _,
               out var blobClientMock);

          var content = new MemoryStream(fixture.CreateMany<byte>(64).ToArray());
          var fileName = fixture.Create<string>() + ".png";
          var contentType = "image/png";

          // Act
          await sut.StoreFileAsync(content, fileName, contentType, CancellationToken.None);

          // Assert
          blobClientMock.Verify(
               b => b.UploadAsync(
                    content,
                    It.IsAny<BlobUploadOptions>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task StoreFileAsync_ValidInput_SetsCorrectContentTypeHeader()
     {
          // Arrange
          var fixture = new Fixture();
          var sut = MakeSut(
               out _,
               out _,
               out var blobClientMock);

          var content = new MemoryStream(fixture.CreateMany<byte>(64).ToArray());
          var fileName = fixture.Create<string>() + ".pdf";
          const string contentType = "application/pdf";

          // Act
          await sut.StoreFileAsync(content, fileName, contentType, TestContext.Current.CancellationToken);

          // Assert
          blobClientMock.Verify(
               b => b.UploadAsync(
                    It.IsAny<Stream>(),
                    It.Is<BlobUploadOptions>(o =>
                         o.HttpHeaders != null &&
                         o.HttpHeaders.ContentType == contentType),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task StoreFileAsync_ValidInput_SetsFileNameAndContentTypeInMetadata()
     {
          // Arrange
          var fixture = new Fixture();
          var sut = MakeSut(
               out _,
               out _,
               out var blobClientMock);

          var content = new MemoryStream(fixture.CreateMany<byte>(64).ToArray());
          var fileName = fixture.Create<string>() + ".jpg";
          const string contentType = "image/jpeg";

          // Act
          await sut.StoreFileAsync(content, fileName, contentType, TestContext.Current.CancellationToken);

          // Assert
          blobClientMock.Verify(
               b => b.UploadAsync(
                    It.IsAny<Stream>(),
                    It.Is<BlobUploadOptions>(o =>
                         o.Metadata != null &&
                         o.Metadata["FileName"] == fileName &&
                         o.Metadata["ContentType"] == contentType),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task StoreFileAsync_ValidInput_UsesCorrectContainerName()
     {
          // Arrange
          var fixture = new Fixture();
          var containerName = fixture.Create<string>();
          var options = new BlobStorageOptions
          {
               ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=dGVzdA==;EndpointSuffix=core.windows.net",
               ContainerName = containerName
          };

          var sut = MakeSut(
               out var blobServiceClientMock,
               out _,
               out _,
               options);

          var content = new MemoryStream(fixture.CreateMany<byte>(32).ToArray());

          // Act
          await sut.StoreFileAsync(content, fixture.Create<string>() + ".jpg", "image/jpeg", TestContext.Current.CancellationToken);

          // Assert
          blobServiceClientMock.Verify(
               s => s.GetBlobContainerClient(containerName),
               Times.AtLeastOnce);
     }

     [Fact]
     public async Task StoreFileAsync_FirstCall_CreatesContainerIfNotExists()
     {
          // Arrange
          var fixture = new Fixture();
          var sut = MakeSut(
               out _,
               out var blobContainerClientMock,
               out _);

          var content = new MemoryStream(fixture.CreateMany<byte>(32).ToArray());

          // Act
          await sut.StoreFileAsync(content, fixture.Create<string>() + ".jpg", "image/jpeg", TestContext.Current.CancellationToken);

          // Assert
          blobContainerClientMock.Verify(
               c => c.CreateIfNotExistsAsync(
                    PublicAccessType.Blob,
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<BlobContainerEncryptionScopeOptions>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task StoreFileAsync_UploadFails_PropagatesException()
     {
          // Arrange
          var fixture = new Fixture();
          var sut = MakeSut(
               out _,
               out _,
               out var blobClientMock);

          blobClientMock
               .Setup(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new RequestFailedException("Upload failed"));

          var content = new MemoryStream(fixture.CreateMany<byte>(32).ToArray());

          // Act
          var act = async () => await sut.StoreFileAsync(content, fixture.Create<string>() + ".jpg", "image/jpeg");

          // Assert
          await act.Should().ThrowAsync<RequestFailedException>();
     }

     [Fact]
     public async Task StoreFileAsync_ValidInput_BlobNamePreservesOriginalExtension()
     {
          // Arrange
          var fixture = new Fixture();
          var sut = MakeSut(
               out _,
               out var blobContainerClientMock,
               out _);

          var content = new MemoryStream(fixture.CreateMany<byte>(32).ToArray());
          const string fileName = "my-photo.jpeg";

          // Act
          await sut.StoreFileAsync(content, fileName, "image/jpeg", TestContext.Current.CancellationToken);

          // Assert
          blobContainerClientMock.Verify(
               c => c.GetBlobClient(It.Is<string>(name => name.EndsWith(".jpeg"))),
               Times.Once);
     }

     [Fact]
     public async Task StoreFileAsync_ValidInput_BlobNameIsUnique()
     {
          // Arrange
          var fixture = new Fixture();
          var capturedBlobNames = new List<string>();

          var sut = MakeSut(
               out _,
               out var blobContainerClientMock,
               out _);

          blobContainerClientMock
               .Setup(c => c.GetBlobClient(It.IsAny<string>()))
               .Callback<string>(name => capturedBlobNames.Add(name))
               .Returns(Mock.Of<BlobClient>(b =>
                    b.Uri == new Uri("https://test.blob.core.windows.net/container/blob") &&
                    b.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()) == Task.FromResult(Mock.Of<Response<BlobContentInfo>>())));

          var content1 = new MemoryStream(fixture.CreateMany<byte>(32).ToArray());
          var content2 = new MemoryStream(fixture.CreateMany<byte>(32).ToArray());

          // Act
          await sut.StoreFileAsync(content1, "file1.jpg", "image/jpeg", TestContext.Current.CancellationToken);
          await sut.StoreFileAsync(content2, "file2.jpg", "image/jpeg", TestContext.Current.CancellationToken);

          // Assert
          capturedBlobNames.Should().HaveCount(2);
          capturedBlobNames[0].Should().NotBe(capturedBlobNames[1]);
     }
}
