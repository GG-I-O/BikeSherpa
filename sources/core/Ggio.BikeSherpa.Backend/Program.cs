using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Features.Courses;
using Ggio.DddCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Auth0.AspNetCore.Authentication.Api;
using Ggio.BikeSherpa.Backend.Features.Courses.Get;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add API services
builder.Services.AddFastEndpoints();

// Add OpenAPI & Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
     options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});
builder.Services.AddOpenApiDocument();

// Add connection to the database
var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDddDbContext<BackendDbContext>((_, options) =>
     options.UseNpgsql(connectionString)
);
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<BackendDbContext>());

// Injection
builder.Services.ConfigureCourseFeature();
builder.Services.AddBackendDomain();

// Add DDD infrastructure services
builder.Services.AddInfrastructureServices();

// Add Mediator for domain event dispatching
builder.Services.AddMediator(options =>
{
     options.Assemblies = [typeof(GetCourseQuery).Assembly];
     options.ServiceLifetime = ServiceLifetime.Scoped;
});

// Authentification
builder.Services.AddAuthorization();
builder.Services.AddAuth0ApiAuthentication(options =>
{
     options.Domain = builder.Configuration["Auth0Domain"];
     options.JwtBearerOptions = new JwtBearerOptions
     {
          Audience = builder.Configuration["Auth0Identifier"]
     };
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
     app.MapOpenApi();
     app.UseOpenApi((options =>
     {
          options.Path = "/swagger/v1/swagger.json";
     }));
     app.UseSwaggerUi();
}

await app.RunAsync();
