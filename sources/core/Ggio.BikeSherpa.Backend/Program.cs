using FastEndpoints;
using FastEndpoints.Swagger;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Features.Courses;
using Ggio.DddCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Auth0.AspNetCore.Authentication.Api;
using Ggio.BikeSherpa.Backend.Features.Courses.Get;
using Ggio.BikeSherpa.Backend.Features.Notification;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.DddCore;
using Ggio.DddCore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
     });

// Add connection to the database
var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDddDbContext<BackendDbContext>((_, options) =>
     options.UseNpgsql(connectionString)
);

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<BackendDbContext>());

// Injection
builder.Services.ConfigureCourseFeature();
builder.Services.AddBackendDomain();

// Notification
builder.Services.AddScoped<IResourceNotificationService, ResourceNotificationService>();

// Add DDD infrastructure services
builder.Services.AddInfrastructureServices();

// Add Mediator for domain event dispatching
builder.Services.AddMediator(options =>
{
     options.Assemblies = [typeof(GetCourseQuery).Assembly, typeof(EntityBase).Assembly, typeof(EfCoreDomainEntityAddedEventHandler)];
     options.ServiceLifetime = ServiceLifetime.Scoped;
});

// Authentification
builder.Services.AddAuthorization();
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
          }
     };
});

// Logger
builder.Host.UseSerilog((context, configuration) =>
{
     configuration.ReadFrom.Configuration(context.Configuration);
     configuration.WriteTo.Console();
     configuration.WriteTo.GrafanaLoki("http://localhost:3100"); //TODO : créer un setting dans appsettings.Developpement.json et le lire depuis la configuration
});
builder.Services.AddHttpLogging(o => { });

var app = builder.Build();

app.MapHub<ResourceNotificationHub>("/hubs/notifications");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints()
     .UseSwaggerGen();

app.UseResourceNotifications();

app.UseSerilogRequestLogging();
app.UseHttpLogging();

await app.RunAsync();
