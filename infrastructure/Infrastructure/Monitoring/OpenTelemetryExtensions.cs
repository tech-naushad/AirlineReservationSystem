using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Infrastructure.Monitoring
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetryMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            var meterName = configuration.GetValue<string>("MeterName")
                ?? throw new Exception("Unable to locate Otel meter name.");

            var otelEndpoint = configuration["Otel:Endpoint"]
                ?? throw new Exception("Otel endpoint was not configured.");

            services.AddOpenTelemetry()
                .WithMetrics(opt =>
                {
                    opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BookingService"))
                        .AddMeter(meterName)
                        .AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddProcessInstrumentation()
                        .AddOtlpExporter(opts =>
                        {
                            opts.Endpoint = new Uri(otelEndpoint);
                        })
                        .AddPrometheusExporter(); // Adding Prometheus Exporter
                });

            return services;
        }
    }
}
