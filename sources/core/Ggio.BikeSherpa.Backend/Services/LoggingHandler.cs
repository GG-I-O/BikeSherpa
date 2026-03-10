namespace Ggio.BikeSherpa.Backend.Services;

public class LoggingHandler : DelegatingHandler
{
     public LoggingHandler() : base(new HttpClientHandler()) {}

     override protected async Task<HttpResponseMessage> SendAsync(
          HttpRequestMessage request,
          CancellationToken cancellationToken)
     {
          Console.WriteLine("=========================================");
          Console.WriteLine("=============== REQUEST =================");
          Console.WriteLine("=========================================");
          Console.WriteLine(request);

          if (request.Content != null)
          {
               var body = await request.Content.ReadAsStringAsync(cancellationToken);
               Console.WriteLine(body);
          }

          var response = await base.SendAsync(request, cancellationToken);

          Console.WriteLine("=========================================");
          Console.WriteLine("============= RESPONSE ==================");
          Console.WriteLine("=========================================");

          if (response.Content != null)
          {
               var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
               Console.WriteLine(responseBody);
          }

          return response;
     }
}
