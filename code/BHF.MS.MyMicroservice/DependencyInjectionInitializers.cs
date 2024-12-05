using Azure.Core;
using Azure.Identity;
using BHF.MS.new8.HealthCheck;
using BHF.MS.new8.Models.Settings;
using BHF.MS.new8.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace BHF.MS.new8
{
    [ExcludeFromCodeCoverage(Justification = "It's an IoC initializer")]
    public static class DependencyInjectionInitializers
    {
        public static void AddOptionsConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.AddOptions<ServiceSettings>()
                .Bind(builder.Configuration.GetSection("ServiceSettings"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            builder.Services.AddOptions<HealthCheckSettings>()
                .Bind(builder.Configuration.GetSection("HealthCheck"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        public static void AddHealthCheckConfiguration(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHealthChecks()
                .AddCheck<ExampleServiceHealthCheck>(
                    name: $"{nameof(ExampleServiceHealthCheck)} health check",
                    tags: ["ready"]);
        }

        public static void AddCustomServices(IServiceCollection serviceCollection)
        {
            Database.DependencyInjectionInitializers.AddDatabases(serviceCollection);
            Database.DependencyInjectionInitializers.AddCustomServices(serviceCollection);

            serviceCollection.AddTransient<ExampleServiceHealthCheck>();
            serviceCollection.AddTransient<IExampleService, ExampleService>();

            AddHttpClients(serviceCollection);
        }

        private static void AddHttpClients(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient<IExampleService, ExampleService>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ServiceSettings>>().Value;
                client.BaseAddress = new Uri(options.HttpClient.BaseAddress);
            });
        }

        public static void MapHealthChecks(WebApplication app)
        {
            app.MapHealthChecks("/healthz/ready",
                new HealthCheckOptions { Predicate = healthCheck => healthCheck.Tags.Contains("ready") });

            app.MapHealthChecks("/healthz/live", new HealthCheckOptions { Predicate = _ => false });
        }

        public static void AddKeyVaultIntegration(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("KeyVaultUri");
            if (string.IsNullOrEmpty(connectionString))
            {
                return;
            }

            var keyVaultUri = new Uri(connectionString);
            TokenCredential credentials;
            if (builder.Environment.IsDevelopment())
            {
                credentials = new ClientSecretCredential(builder.Configuration.GetConnectionString("TenantId"),
                    builder.Configuration.GetConnectionString("ClientId"),
                    builder.Configuration.GetConnectionString("ClientSecret"));
            }
            else
            {
                credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = builder.Configuration.GetConnectionString("ManagedIdentityClientId")
                });
            }

            builder.Configuration.AddAzureKeyVault(
                keyVaultUri,
                credentials);
        }

        public static void AddApplicationInsights(WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                return;
            }

            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.AddServiceProfiler();
        }
    }
}

