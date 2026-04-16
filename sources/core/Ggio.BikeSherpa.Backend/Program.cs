using System.Security.Claims;
using Auth0.AspNetCore.Authentication.Api;
using FastEndpoints;
using FastEndpoints.Swagger;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Features.Couriers;
using Ggio.BikeSherpa.Backend.Features.Customers;
using Ggio.BikeSherpa.Backend.Features.Deliveries;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Ggio.BikeSherpa.Backend.Services.Middleware;
using Ggio.BikeSherpa.Backend.Services.Notification;
using Ggio.DddCore;
using Ggio.DddCore.Infrastructure;
using Ggio.DddCore.Infrastructure.Persistence;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NJsonSchema.Generation;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

// Add API services
if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
     builder.Services.AddFastEndpoints()
          .SwaggerDocument(options =>
          {
               options.DocumentSettings = settings =>
               {
                    settings.Title = "Bike Sherpa API";
                    settings.Version = "v1";
                    settings.SchemaSettings.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
                    settings.SchemaSettings.GenerateXmlObjects = true;
               };

               options.ShortSchemaNames = true;
               options.AutoTagPathSegmentIndex = 0;
          });
}

// Backend infrastructure layer dependencies
builder.Services.AddBackendInfrastructure(builder.Configuration);

// Injection
builder.Services.ConfigureDeliveryFeature();
builder.Services.ConfigureCustomerFeature();
builder.Services.ConfigureCourierFeature();
builder.Services.AddBackendDomain();

// Notification
builder.Services.AddSignalR(config => { config.EnableDetailedErrors = true; });
builder.Services.AddScoped<IResourceNotificationService, ResourceNotificationService>();

// Hateoas
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHateoasService, HateoasService>();

// Add DDD infrastructure services
builder.Services.AddDddInfrastructureServices();

// Add Mediator for domain event dispatching
builder.Services.AddMediator(options =>
{
     options.Assemblies = [typeof(GetDeliveryQuery).Assembly, typeof(EntityBase).Assembly, typeof(EfCoreDomainEntityAddedEventHandler)];
     options.ServiceLifetime = ServiceLifetime.Scoped;
     options.NotificationPublisherType = typeof(TaskWhenAllPublisher);
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
if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
     builder.Services.AddAuthorization(options =>
     {
          const string scopeName = "scope";
          options.AddPolicy("read:customers", policy => policy.RequireClaim(scopeName, "read:customers"));
          options.AddPolicy("write:customers", policy => policy.RequireClaim(scopeName, "write:customers"));
          options.AddPolicy("read:couriers", policy => policy.RequireClaim(scopeName, "read:couriers"));
          options.AddPolicy("write:couriers", policy => policy.RequireClaim(scopeName, "write:couriers"));
          options.AddPolicy("read:deliveries", policy => policy.RequireClaim(scopeName, "read:deliveries"));
          options.AddPolicy("write:deliveries", policy => policy.RequireClaim(scopeName, "write:deliveries"));
     });

     builder.Services.AddAuth0ApiAuthentication(options =>
     {
          options.Domain = builder.Configuration["Auth0Domain"];
          options.JwtBearerOptions = new JwtBearerOptions
          {
               Audience = builder.Configuration["Auth0Identifier"],
               RequireHttpsMetadata = true,
               MetadataAddress = $"{builder.Configuration["Auth0Metadata"]}",
               TokenValidationParameters = new TokenValidationParameters
               {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Auth0Issuer"],
                    ValidAudience = builder.Configuration["Auth0Identifier"]
               },
               Events = new JwtBearerEvents
               {
                    OnTokenValidated = async context =>
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
}


var app = builder.Build();
app.MapHub<ResourceNotificationHub>("/hubs/notifications");

app.UseCors();
app.UseHttpsRedirection();
app.UsePathBase("/api");
app.UseAuthentication();
app.UseAuthorization();


if (!app.Environment.IsEnvironment("IntegrationTest"))
{
     app.UseFastEndpoints(config => { config.Endpoints.ShortNames = true; })
          .UseSwaggerGen();

     app.UseSerilogRequestLogging();
     app.UseHttpLogging();
}

app.UseOperationIdMiddleware();
app.UseResourceNotifications();
app.UseValidationExceptionMiddleware();

await app.RunAsync();
