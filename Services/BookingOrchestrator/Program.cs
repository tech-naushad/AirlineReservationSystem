using BookingOrchestrator.Consumers;
using BookingOrchestrator.Orchestrator;
using BookingOrchestrator.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BookingOrchestratorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();
builder.Services.AddScoped<DBIntializer>();

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<BookingSagaOrchestrator, BookingState>()
    .EntityFrameworkRepository(r =>
    {
        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
        r.AddDbContext<DbContext, BookingOrchestratorDbContext>((provider, options) =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
    });

    x.AddConsumer<BookingCreatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://host.docker.internal"), host =>
        {
            host.Username("admin");
            host.Password("admin");
            cfg.ConfigureEndpoints(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<DBIntializer>();  
    await initialiser.InitialiseAsync();
}

app.Run();
