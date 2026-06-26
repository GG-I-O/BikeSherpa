namespace Ggio.BikeSherpa.Backend.Infrastructure.Storage;

public interface IExportSaveService
{
     public Task<string> SaveCourierReportAsync(string fileName, byte[] fileContent, string contentType, CancellationToken cancellationToken);
     
}
