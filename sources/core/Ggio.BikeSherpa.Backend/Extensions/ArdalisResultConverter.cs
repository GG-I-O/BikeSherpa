using Ardalis.Result;
using FastEndpoints;
using IResult = Ardalis.Result.IResult;

namespace Ggio.BikeSherpa.Backend.Extensions;

public static class ArdalisResultConverter
{
     public async static Task ToEndpointResult<TRequest>(this ResponseSender<TRequest, Result> sender,
          Result result,
          CancellationToken ct) where TRequest : notnull
     {
          switch (result.Status)
          {
               case ResultStatus.Ok:
                    await sender.OkAsync(result.Value, ct);
                    break;
               case ResultStatus.Created:
                    throw new InvalidOperationException("Created is not supported, you should use CreatedAtAsync inside Endpoint");
               case ResultStatus.NotFound:
                    await sender.NotFoundAsync(ct);
                    break;
               case ResultStatus.Unauthorized:
                    await sender.UnauthorizedAsync(ct);
                    break;
               case ResultStatus.NoContent:
                    await sender.NoContentAsync(ct);
                    break;
               case ResultStatus.Invalid:
               case ResultStatus.Unavailable:
               case ResultStatus.CriticalError:
               case ResultStatus.Conflict:
               case ResultStatus.Error:
               case ResultStatus.Forbidden:
               default:
                    await sender.ErrorsAsync(cancellation: ct);
                    break;
          }
     }

     extension<TRequest>(ResponseSender<TRequest, object?> sender) where TRequest : notnull
     {
          public async Task ToEndpointResult(Result result,
               CancellationToken ct)
          {
               await sender.ToEndpointWithoutRequestResult(result, ct);
          }

          public async Task ToEndpointWithoutRequestResult(IResult result,
               CancellationToken ct)
          {
               switch (result.Status)
               {
                    case ResultStatus.Ok:
                         await sender.OkAsync(result.GetValue(), ct);
                         break;
                    case ResultStatus.Created:
                         throw new InvalidOperationException("Created is not supported, you should use CreatedAtAsync inside Endpoint");
                    case ResultStatus.NotFound:
                         await sender.NotFoundAsync(ct);
                         break;
                    case ResultStatus.Unauthorized:
                         await sender.UnauthorizedAsync(ct);
                         break;
                    case ResultStatus.NoContent:
                         await sender.NoContentAsync(ct);
                         break;
                    case ResultStatus.Invalid:
                    case ResultStatus.Unavailable:
                    case ResultStatus.CriticalError:
                    case ResultStatus.Conflict:
                    case ResultStatus.Error:
                    case ResultStatus.Forbidden:
                    default:
                         await sender.ErrorsAsync(cancellation: ct);
                         break;
               }
          }
     }
}
