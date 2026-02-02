using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Services;

public abstract class TestWebApplicationFactory(string policyName, string scope) : WebApplicationFactory<Program>
{
     public Mock<IMediator> MockMediator { get; } = new();

     private string PolicyName { get; } = policyName;
     private string Scope { get; } = scope;

     override protected void ConfigureWebHost(IWebHostBuilder builder)
     {
          builder.UseEnvironment("IntegrationTest");

          builder.ConfigureServices(services =>
          {
               services.AddFastEndpoints(config =>
               {
                    config.Assemblies = [typeof(Program).Assembly];
                    config.SourceGeneratorDiscoveredTypes = [];
                    config.Filter = type => type.Assembly == typeof(Program).Assembly;
               });

               services.AddSingleton(MockMediator.Object);

               services
                    .AddAuthentication("Test")
                    .AddScheme<TestAuthSchemeOptions, TestAuthHandler>("Test", options => options.Scope = Scope);

               services.AddAuthorization(options =>
               {
                    options.AddPolicy(PolicyName,
                         policy => policy.RequireClaim("scope", Scope));
               });
          });

          builder.Configure(app =>
          {
               app.UsePathBase("/api");
               app.UseRouting();
               app.UseAuthentication();
               app.UseAuthorization();
               app.UseEndpoints(endpoints => endpoints.MapFastEndpoints());
          });
     }
}
