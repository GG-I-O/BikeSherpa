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
using Ggio.BikeSherpa.Backend.Infrastructure.Interceptors;
using Ggio.BikeSherpa.Backend.Services;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Ggio.BikeSherpa.Backend.Services.Notification;
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

          options.ShortSchemaNames = true;
     });

// Add connection to the database
var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDddDbContext<BackendDbContext>((_, options) =>
     options.UseNpgsql(connectionString)
          .AddInterceptors(new DateInterceptor())
);

builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<BackendDbContext>());

// Injection
builder.Services.ConfigureCourseFeature();
builder.Services.ConfigureClientFeature();
builder.Services.AddBackendDomain();

// Notification
builder.Services.AddSignalR();
builder.Services.AddScoped<IResourceNotificationService, ResourceNotificationService>();

// Hateoas
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHateoasService, HateoasService>();

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
     configuration.WriteTo.GrafanaLoki(builder.Configuration["GrafanaLoki"] ?? "http://localhost:3100");
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
