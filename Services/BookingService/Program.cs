using BookingService.SagaStateMachine;
using BookingService.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using BookingService.Consumers;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
string serviceName = "BookingService";

// Add services to the container.
builder.Services.AddDbContext<BookingStateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddScoped<DBIntializer>();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<BookingStateMachine, BookingState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
            r.AddDbContext<DbContext, BookingStateDbContext>((provider, options) =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
        });

    x.AddConsumer<BookingConfirmedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://host.docker.internal"), host =>
        {
            host.Username("admin");
            host.Password("admin");
            cfg.ConfigureEndpoints(context);
        });
         
        // Manually instrumenting MassTransit with OpenTelemetry
        //cfg.UseMessageData<OpenTelemetryMessageData>(); // Use OpenTelemetry on each message
    });
});

// Add OpenTelemetry tracing
//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracerProviderBuilder =>
//        tracerProviderBuilder
//            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            .AddSqlClientInstrumentation()
//            .AddOtlpExporter(otlpOptions =>
//            {
//                otlpOptions.Endpoint = new Uri("http://otel-collector:4317"); // OpenTelemetry Collector
//                otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
//            })

//            .AddSource("MassTransit")  // Manually adding MassTransit source
//    )
//    .WithMetrics(metricsProviderBuilder =>
//        metricsProviderBuilder
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            .AddMeter("BookingService")
//            .AddOtlpExporter(otlpOptions =>
//            {
//                otlpOptions.Endpoint = new Uri("http://otel-collector:4317");
//                otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
//            }));


builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder.AddAspNetCoreInstrumentation()
                    //.AddAspNetCoreInstrumentation()
                    //.AddHttpClientInstrumentation()
                    //.AddSqlClientInstrumentation()
                    .AddSource("MassTransit");

        tracerProviderBuilder.AddOtlpExporter(options => options.Endpoint = new Uri("http://jaeger:4317"));
    })
    .WithMetrics(metricsProviderBuilder =>
    {
        metricsProviderBuilder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
        metricsProviderBuilder//.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel")
                    //.AddAspNetCoreInstrumentation()
                    //.AddHttpClientInstrumentation()
                    .AddMeter("BookingService");
        metricsProviderBuilder.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10]
            });

        metricsProviderBuilder.AddPrometheusExporter();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<DBIntializer>();
    await initialiser.InitialiseAsync();
}

app.UseHttpsRedirection();

app.MapPrometheusScrapingEndpoint();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
