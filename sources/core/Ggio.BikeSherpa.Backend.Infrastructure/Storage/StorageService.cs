using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Storage;

public partial class StorageService : IDeliveryStepAttachmentSaveService
{
     private readonly static SemaphoreSlim Lock = new(1);
     private readonly BlobServiceClient _blobServiceClient;
     private readonly ILogger<StorageService> _logger;
     private readonly BlobStorageOptions _options;
     private volatile bool _containerChecked;

     public StorageService(IOptions<BlobStorageOptions> options, ILogger<StorageService> logger)
          : this(options, logger, new BlobServiceClient(options.Value.ConnectionString))
     {
     }

     internal StorageService(IOptions<BlobStorageOptions> options, ILogger<StorageService> logger, BlobServiceClient blobServiceClient)
     {
          _logger = logger;
          _options = options.Value;
          _blobServiceClient = blobServiceClient;
     }

     public async Task<string> StoreFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
     {
          await EnsureContainerExistsAsync(cancellationToken);

          var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
          var extension = Path.GetExtension(fileName);
          var blobName = $"{Guid.NewGuid().ToString()}{extension}";
          var blobClient = containerClient.GetBlobClient(blobName);

          var uploadOptions = new BlobUploadOptions
          {
               HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
               Metadata = new Dictionary<string, string>
               {
                    { "FileName", fileName },
                    { "ContentType", contentType }
               }
          };

          LogStoringNewBlobIntoStorage(blobName, fileName, contentType);

          await blobClient.UploadAsync(content, uploadOptions, cancellationToken);

          return blobClient.Uri.ToString();
     }

     private async Task EnsureContainerExistsAsync(CancellationToken cancellationToken)
     {

          await Lock.WaitAsync(cancellationToken);
          if (_containerChecked)
          {
               return;
          }

          try
          {
               LogEnsuringThatTheContainerContainerExists(_options.ContainerName);
               var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
               await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
               _containerChecked = true;
          }
          finally
          {
               Lock.Release();
          }
     }

     [LoggerMessage(LogLevel.Information, "Ensuring that the container {ContainerName} exists ")]
     partial void LogEnsuringThatTheContainerContainerExists(string containerName);

     [LoggerMessage(LogLevel.Information, "Storing new Blob {BlobName} into storage from {FileName} of content type {ContentType}")]
     partial void LogStoringNewBlobIntoStorage(string blobName, string fileName, string contentType);
}
