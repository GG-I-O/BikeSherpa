using FastEndpoints;
using Ggio.BikeSherpa.BackendSaaS.Features.Courses;
using Ggio.BikeSherpa.BackendSaaS.Features.Courses.Get;
using Ggio.DddCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mediator;
using Auth0.AspNetCore.Authentication.Api;
using Ggio.BikeSherpa.Backend.Infrastructure;
using Ggio.BikeSherpa.Backend.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddFastEndpoints();


// Add DDD infrastructure services
builder.Services.AddInfrastructureServices();

// Add connection to database
var conStrBuilder = new NpgsqlConnectionStringBuilder(
     builder.Configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Postgres connection string not found"))
{
     Password = builder.Configuration["DbPassword"]
};

var connection = conStrBuilder.ConnectionString;
builder.Services.AddDbContext<BackendDbContext>(options =>
     options.UseNpgsql(connection)
);

// Add Mediator for domain event dispatching
builder.Services.AddMediator(options =>
{
     options.Assemblies = [typeof(GetCourseQuery).Assembly];
     options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.ConfigureCourseFeature();

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
