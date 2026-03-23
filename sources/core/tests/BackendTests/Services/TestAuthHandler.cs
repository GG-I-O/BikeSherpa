using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BackendTests.Services;

public class TestAuthSchemeOptions : AuthenticationSchemeOptions
{
     public string? Scope { get; set; }
}

public class TestAuthHandler(
     IOptionsMonitor<TestAuthSchemeOptions> options,
     ILoggerFactory logger,
     UrlEncoder encoder)
     : AuthenticationHandler<TestAuthSchemeOptions>(options, logger, encoder)
{
     override protected Task<AuthenticateResult> HandleAuthenticateAsync()
     {
          var identity = new ClaimsIdentity("Test");
          if (Options.Scope != null)
          {
               identity.AddClaim(new Claim("scope", Options.Scope));
          }

          var principal = new ClaimsPrincipal(identity);
          var ticket = new AuthenticationTicket(principal, "Test");

          return Task.FromResult(AuthenticateResult.Success(ticket));
     }
}
