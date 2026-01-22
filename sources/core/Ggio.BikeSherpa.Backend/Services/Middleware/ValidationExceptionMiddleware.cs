using FluentValidation;
using Ggio.BikeSherpa.Backend.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Services.Middleware;

public class ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
{
     public async Task InvokeAsync(HttpContext context)
     {
          try
          {
               await next(context);
          }
          catch (ValidationException exception)
          {
               logger.LogDebug(exception, "A validation exception occurred: {Errors}",
                    exception.Errors.Select(e => e.ErrorMessage)
                         .Aggregate((a, b) => $"{a}, {b}"));

               List<ThrownValidationError> errors = [];
               foreach (var error in exception.Errors)
               {
                    errors.Add(new ThrownValidationError
                    {
                         Origin = error.PropertyName,
                         Message = error.ErrorMessage
                    });
               }

               var result = Results.BadRequest(errors);
               await result.ExecuteAsync(context);
          }
     }
}

public static class ValidationExceptionExtensions
{
     extension(IApplicationBuilder builder)
     {
          public IApplicationBuilder UseValidationExceptionMiddleware() => builder.UseMiddleware<ValidationExceptionMiddleware>();
     }
}
