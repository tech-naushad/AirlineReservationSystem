using MassTransit;
using Microsoft.EntityFrameworkCore;
using SeatReservationService.Consumers;
using SeatReservationService.Persistence;
using Infrastructure.MassTransit;
using Infrastructure.Observability;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<DBIntializer>();

builder.Services.AddDbContext<ReservationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReservationDatabase")));

builder.Services.ConfigureMassTransit(builder.Configuration,
    typeof(SeatReservationConsumer)
//typeof(PaymentFailedConsumer) // Add more consumers here
);

builder.Services.AddOpenTelemetryMetrics(builder.Configuration);
builder.Services.AddOpenTelemetryTracing(builder.Configuration, "ReservationService");

var app = builder.Build();

// Configure the HTTP request pipeline.
// Initialise and database
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<DBIntializer>();
    await initialiser.InitialiseAsync();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
