using BookingService.Consumers;
using BookingService.Persistence;
using BookingService.SagaStateMachine;
using Infrastructure.MassTransit;
using Infrastructure.Observability;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BookingStateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookingDatabase")));

builder.Services.AddControllers();
builder.Services.AddScoped<DBIntializer>();
builder.Services.AddSwaggerGen();

//Configure MassTransit with Saga and multiple consumers
builder.Services.ConfigureMassTransit<BookingStateMachine, BookingState, BookingStateDbContext>(
    builder.Configuration,
    "BookingDatabase", 
    typeof(BookingConfirmedConsumer) // Add more consumers as needed
);

builder.Services.AddOpenTelemetryMetrics(builder.Configuration);
builder.Services.AddOpenTelemetryTracing(builder.Configuration, "BookingService");

var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<DBIntializer>();
    await initialiser.InitialiseAsync();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
