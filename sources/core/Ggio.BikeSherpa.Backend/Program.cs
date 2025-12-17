using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Swagger;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Features.Courses;
using Ggio.DddCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Auth0.AspNetCore.Authentication.Api;
using Ggio.BikeSherpa.Backend.Features.Courses.Get;
using Ggio.BikeSherpa.Backend.Features.Customers;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;
using Ggio.DddCore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

// Add API services
builder.Services.AddFastEndpoints()
     .SwaggerDocument(options =>
     {
          options.DocumentSettings = settings =>
          {
               settings.Title = "Bike Sherpa API";
               settings.Version = "v1";
          };

          options.ShortSchemaNames = true;
     });

// Backend infrastructure layer dependencies
builder.Services.AddBackendInfrastructure(builder.Configuration);

// Injection
builder.Services.ConfigureCourseFeature();
builder.Services.ConfigureCustomerFeature();
builder.Services.AddBackendDomain();

// Notification
builder.Services.AddSignalR(config =>
{
     config.EnableDetailedErrors = true;
});
builder.Services.AddScoped<IResourceNotificationService, ResourceNotificationService>();

// Hateoas
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHateoasService, HateoasService>();

// Add DDD infrastructure services
builder.Services.AddDddInfrastructureServices();

// Add Mediator for domain event dispatching
builder.Services.AddMediator(options =>
{
     options.Assemblies = [typeof(GetCourseQuery).Assembly, typeof(EntityBase).Assembly, typeof(EfCoreDomainEntityAddedEventHandler)];
     options.ServiceLifetime = ServiceLifetime.Scoped;
});

// Cors
var allowedOrigins = (builder.Configuration["CORS:AllowedOrigins"] ?? "").Split(',');
builder.Services.AddCors(options =>
{
     options.AddDefaultPolicy(policy =>
     {
          policy.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
     });
});

// Authentification
builder.Services.AddAuthorization(options =>
{
     options.AddPolicy("read:customers", policy => policy.RequireClaim("scope", "read:customers"));
     options.AddPolicy("write:customers", policy => policy.RequireClaim("scope","write:customers"));
});
builder.Services.AddAuth0ApiAuthentication(options =>
{
     options.Domain = builder.Configuration["Auth0Domain"];
     options.JwtBearerOptions = new JwtBearerOptions
     {
          Audience = builder.Configuration["Auth0Identifier"],
          RequireHttpsMetadata = true,
          MetadataAddress = $"{builder.Configuration["Auth0Metadata"]}",
          TokenValidationParameters = new TokenValidationParameters()
          {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = builder.Configuration["Auth0Issuer"],
               ValidAudience = builder.Configuration["Auth0Identifier"]
          },
          Events = new JwtBearerEvents()
          {
               OnTokenValidated = async (context) =>
               {
                    if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
                    {
                         var scopeClaims = claimsIdentity.FindFirst("scope");
                         if (scopeClaims is not null)
                         {
                              claimsIdentity.RemoveClaim(scopeClaims);
                              claimsIdentity.AddClaims(scopeClaims.Value.Split(' ').Select(scope => new Claim("scope", scope)));
                         }
                    }
                    await Task.CompletedTask;
               }
          }
     };
});

// Logger
builder.Host.UseSerilog((context, configuration) =>
{
     configuration.ReadFrom.Configuration(context.Configuration);
     configuration.WriteTo.GrafanaLoki(builder.Configuration["GrafanaLoki"]!);
});
builder.Services.AddHttpLogging();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ResourceNotificationHub>("/hubs/notifications");

app.UseFastEndpoints(config =>
     {
          config.Endpoints.ShortNames = true;
     })
     .UseSwaggerGen();

app.UseResourceNotifications();

app.UseSerilogRequestLogging();
app.UseHttpLogging();

await app.RunAsync();
