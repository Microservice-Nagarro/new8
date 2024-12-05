using System.Diagnostics.CodeAnalysis;

namespace BHF.MS.new8
{
    [ExcludeFromCodeCoverage(Justification = "It's a main Program class")]
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            DependencyInjectionInitializers.AddKeyVaultIntegration(builder);
            DependencyInjectionInitializers.AddOptionsConfiguration(builder);
            DependencyInjectionInitializers.AddCustomServices(builder.Services);

            builder.Services.AddResponseCaching();
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen();

            DependencyInjectionInitializers.AddApplicationInsights(builder);
            Database.DependencyInjectionInitializers.AddHealthCheckConfiguration(builder.Services);
            DependencyInjectionInitializers.AddHealthCheckConfiguration(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.UseResponseCaching();

            app.MapControllers();
            DependencyInjectionInitializers.MapHealthChecks(app);
            if (app.Environment.IsDevelopment())
            {
                Database.DependencyInjectionInitializers.InitializeDatabases(app.Services);
            }

            await app.RunAsync();
        }
    }
}

