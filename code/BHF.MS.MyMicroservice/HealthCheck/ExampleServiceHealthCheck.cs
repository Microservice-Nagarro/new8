using BHF.MS.new8.Models.Settings;
using BHF.MS.new8.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace BHF.MS.new8.HealthCheck
{
    public class ExampleServiceHealthCheck(IExampleService exampleService, ILogger<ExampleServiceHealthCheck> logger, IOptions<HealthCheckSettings> settings) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            for (byte i = 0; i < settings.Value.AttemptsCount; i++)
            {
                var result = await exampleService.GetSomething();
                if (result.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("Successfully connected to the service");
                }

                logger.LogWarning("Failure status code returned: {StatusCode}", (int)result.StatusCode);
            }

            return HealthCheckResult.Unhealthy("Could not connect to the service");
        }
    }
}

