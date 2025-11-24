using FastEndpoints;
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

// Add Mediator for domain event dispatching
builder.Services.AddMediator(options =>
{
     options.Assemblies = [typeof(GetCourseQuery).Assembly];
     options.ServiceLifetime = ServiceLifetime.Scoped;
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.UseFastEndpoints();

app.Run();
