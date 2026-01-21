using Ardalis.Result;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using IResult = Ardalis.Result.IResult;

namespace Ggio.BikeSherpa.Backend.Extensions;

public static class ArdalisResultConverter
{
     public async static Task ToEndpointWithoutRequestResult<TRequest>(this ResponseSender<TRequest, object?> sender,
          IResult result,
          CancellationToken ct) where TRequest : notnull
     {
          switch (result.Status)
          {
               case ResultStatus.Ok:
                    await sender.OkAsync(result.GetValue(), cancellation: ct);
                    break;
               case ResultStatus.Created:
                    throw new InvalidOperationException("Created is not supported, you should use CreatedAtAsync inside Endpoint");
                    break;
               case ResultStatus.NotFound:
                    await sender.NotFoundAsync(cancellation: ct);
                    break;
               case ResultStatus.Unauthorized:
                    await sender.UnauthorizedAsync(cancellation: ct);
                    break;
               case ResultStatus.NoContent:
                    await sender.NoContentAsync(cancellation: ct);
                    break;
               case ResultStatus.Invalid:
               case ResultStatus.Unavailable:
               case ResultStatus.CriticalError:
               case ResultStatus.Conflict:
               case ResultStatus.Error:
               case ResultStatus.Forbidden:
                    await sender.ErrorsAsync(cancellation: ct);
                    break;
          }
     }
     
     public async static Task ToEndpointResult<TRequest>(this ResponseSender<TRequest, Result> sender,
          Result result,
          CancellationToken ct) where TRequest : notnull
     {
          switch (result.Status)
          {
               case ResultStatus.Ok:
                    await sender.OkAsync(result.Value, cancellation: ct);
                    break;
               case ResultStatus.Created:
                    throw new InvalidOperationException("Created is not supported, you should use CreatedAtAsync inside Endpoint");
                    break;
               case ResultStatus.NotFound:
                    await sender.NotFoundAsync(cancellation: ct);
                    break;
               case ResultStatus.Unauthorized:
                    await sender.UnauthorizedAsync(cancellation: ct);
                    break;
               case ResultStatus.NoContent:
                    await sender.NoContentAsync(cancellation: ct);
                    break;
               case ResultStatus.Invalid:
               case ResultStatus.Unavailable:
               case ResultStatus.CriticalError:
               case ResultStatus.Conflict:
               case ResultStatus.Error:
               case ResultStatus.Forbidden:
                    await sender.ErrorsAsync(cancellation: ct);
                    break;
          }
     }
}
