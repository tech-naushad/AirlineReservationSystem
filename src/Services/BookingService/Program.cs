using BookingService.Consumers;
using BookingService.Persistence;
using BookingService.SagaStateMachine;
using Infrastructure.MassTransit;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
string serviceName = "BookingService";

// Add services to the container.
builder.Services.AddDbContext<BookingStateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookingDatabase")));

builder.Services.AddControllers();
builder.Services.AddScoped<DBIntializer>();
builder.Services.AddSwaggerGen();

//Configure MassTransit with Saga and multiple consumers
builder.Services.ConfigureMassTransit<BookingStateMachine, BookingState, BookingStateDbContext>(
    builder.Configuration,
    "BookingDatabase", // Connection string key from appsettings.json
    typeof(BookingConfirmedConsumer) // Add more consumers as needed
);

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
