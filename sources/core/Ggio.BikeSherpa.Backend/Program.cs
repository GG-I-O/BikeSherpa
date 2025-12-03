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
using Microsoft.Extensions.Configuration;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddFastEndpoints();


// Add DDD infrastructure services
builder.Services.AddInfrastructureServices();

// Add connection to the database
var conStrBuilder = new NpgsqlConnectionStringBuilder(
     //TODO utiliser un secret
     builder.Configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Postgres connection string not found"))
{
     Password = builder.Configuration["DbPassword"]
};

var connection = conStrBuilder.ConnectionString;
builder.Services.AddDddDbContext<BackendDbContext>((_, options) =>
     options.UseNpgsql(connection)
);
builder.Services.ConfigureCourseFeature();

builder.Services.AddBackendDomain();
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
     options.Domain = builder.Configuration["Auth0:Domain"];
     options.JwtBearerOptions = new JwtBearerOptions
     {
          Audience = builder.Configuration["Auth0:Audience"]
     };
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

await app.RunAsync();
