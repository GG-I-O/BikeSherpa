using Ardalis.Result;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Extensions;

public abstract class ArdalisResultConverter<TRequest, TResponse> : Endpoint<TRequest, TResponse>
     where TRequest : notnull
{
    protected async Task ConvertToFastEndpointResponse<T>(
         Result<T> result,
         Func<T, TResponse> converter,
         CancellationToken ct)
    {
        switch (result.Status)
        {
            case ResultStatus.Ok:
                await Send.OkAsync(converter(result.Value), cancellation: ct);
                break;
            case ResultStatus.Created:
                await Send.ResultAsync(TypedResults.Created(string.Empty, converter(result.Value)));
                break;
            case ResultStatus.NotFound:
                await Send.NotFoundAsync(cancellation: ct);
                break;
            case ResultStatus.Unauthorized:
                await Send.UnauthorizedAsync(cancellation: ct);
                break;
            case ResultStatus.NoContent:
                await Send.NoContentAsync(cancellation: ct);
                break;
            case ResultStatus.Invalid:
            case ResultStatus.Unavailable:
            case ResultStatus.CriticalError:
            case ResultStatus.Conflict:
            case ResultStatus.Error:
            case ResultStatus.Forbidden:
            default:
                await Send.ErrorsAsync(cancellation: ct);
                break;
        }
    }
}
