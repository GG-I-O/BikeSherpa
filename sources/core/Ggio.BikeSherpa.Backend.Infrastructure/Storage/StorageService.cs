using System.Text.RegularExpressions;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Storage;

public partial class StorageService : IDeliveryStepAttachmentSaveService , IExportSaveService
{
     private const string AttachementDirectory = "attachments";
     private const string CourierReportDirectory = "courier-reports";
     private const string CustomerReportDirectory = "customer-reports";
     
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
          var blobName = $"{AttachementDirectory}/{Guid.NewGuid().ToString()}{extension}";
          var blobClient = await StoreBlobIntoStorage(content, fileName, contentType, cancellationToken, containerClient, blobName);

          return blobClient.Uri.ToString();
     }

     private async Task<BlobClient> StoreBlobIntoStorage(Stream content, string fileName, string contentType, CancellationToken cancellationToken, BlobContainerClient containerClient, string blobName)
     {

          var blobClient = containerClient.GetBlobClient(blobName);

          var uploadOptions = new BlobUploadOptions
          {
               HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
               Metadata = new Dictionary<string, string>
               {
                    { "FileName", RemoveNonAscii(fileName) },
                    { "ContentType", contentType }
               }
          };

          LogStoringNewBlobIntoStorage(blobName, fileName, contentType);

          await blobClient.UploadAsync(content, uploadOptions, cancellationToken);
          return blobClient;
     }
     public static string RemoveNonAscii(string input)
     {
          if (string.IsNullOrEmpty(input)) return input;
    
          // [^\x00-\x7F] matches any character that is NOT in the ASCII range
          return Regex.Replace(input, @"[^\x00-\x7F]", string.Empty);
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
          catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ContainerAlreadyExists)
          {
               _logger.LogDebug(ex,"Container {ContainerName} already exists", _options.ContainerName);
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

     public async Task<string> SaveCourierReportAsync(string fileName, byte[] fileContent, string contentType,  CancellationToken cancellationToken )
     {
          await EnsureContainerExistsAsync(cancellationToken);

          var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
          var extension = Path.GetExtension(fileName);
          var blobName = $"{CourierReportDirectory}/{Guid.NewGuid().ToString()}{extension}";
          using var content = new MemoryStream(fileContent);
          var blobClient = await StoreBlobIntoStorage(content, fileName, contentType, cancellationToken, containerClient, blobName);

          return blobClient.Uri.ToString();
          
     }

     public async Task<string> SaveCustomerReportAsync(string fileName, byte[] fileContent, string contentType, CancellationToken cancellationToken)
     {
          await EnsureContainerExistsAsync(cancellationToken);

          var containerClient = _blobServiceClient.GetBlobContainerClient(_options.ContainerName);
          var extension = Path.GetExtension(fileName);
          var blobName = $"{CustomerReportDirectory}/{Guid.NewGuid().ToString()}{extension}";
          using var content = new MemoryStream(fileContent);
          var blobClient = await StoreBlobIntoStorage(content, fileName, contentType, cancellationToken, containerClient, blobName);

          return blobClient.Uri.ToString();
     }
}
