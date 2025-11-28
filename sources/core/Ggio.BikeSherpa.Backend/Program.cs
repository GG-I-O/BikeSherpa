using FastEndpoints;
using Ggio.BikeSherpa.BackendSaaS.Features.Courses;
using Ggio.BikeSherpa.BackendSaaS.Features.Courses.Get;
using Ggio.DddCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mediator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddFastEndpoints();


// Add DDD infrastructure services
builder.Services.AddInfrastructureServices();
//TODO add connection to database


// Add Mediator for domain event dispatching
builder.Services.AddMediator(options =>
{
     options.Assemblies = [typeof(GetCourseQuery).Assembly];
     options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.ConfigureCourseFeature();


var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.UseFastEndpoints();

await app.RunAsync();
