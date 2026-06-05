using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.Parameters.Get;

public record WorkHourDto
{
    public DateTimeOffset StartDate { get; init; }
    public DateTimeOffset EndDate { get; init; }
}

public class GetParameterWorkHoursEndpoint(
    IParameterRepository parameterRepository
): EndpointWithoutRequest<WorkHourDto>
{
    public override void Configure()
    {
        Get("/general/parameters/workHours");
        Description(x => x.WithTags("general"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var workDays = new WorkHourDto
        {
            StartDate = await parameterRepository.GetWorkStartDateAsync(),
            EndDate = await parameterRepository.GetWorkEndDateAsync()
        };

        await Send.OkAsync(workDays, ct);
    }
}

