using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Infrastructure.Monitoring
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetryMetrics(this IServiceCollection services, 
            IConfiguration configuration)
        {
            var meterName = configuration.GetValue<string>("MeterName")
                ?? throw new Exception("Unable to locate Otel meter name.");

            var otelEndpoint = configuration["Otel:Endpoint"]
                ?? throw new Exception("Otel endpoint was not configured.");

            services.AddOpenTelemetry()
                .WithMetrics(opt =>
                {
                        opt.AddMeter(meterName)
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
        public static IServiceCollection AddOpenTelemetryTracing(this IServiceCollection services,
           IConfiguration configuration, string serviceName)
        {

            var jaegerEndpoint = configuration["Jaeger:Endpoint"]
                ?? throw new Exception("jaeger endpoint was not configured.");

            services.AddOpenTelemetry()                 
                 .WithTracing(tracerProviderBuilder =>
                 {
                     tracerProviderBuilder
                         .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName)) 
                         .AddAspNetCoreInstrumentation(options =>
                         {
                             options.RecordException = true;
                         })
                         .AddHttpClientInstrumentation()
                         .AddSqlClientInstrumentation(options =>
                         {
                             options.SetDbStatementForText = true; // Captures SQL queries not recommnde on prod
                             //options.SetDbStatementForStoredProcedure = true;
                         })
                         .AddOtlpExporter(otlpOptions =>
                         {
                             otlpOptions.Endpoint = new Uri(jaegerEndpoint); // gRPC OTLP Endpoint for Jaeger
                             otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                         });
                 });

            return services;
        }

    }
}
