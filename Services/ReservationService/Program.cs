using MassTransit;
using Microsoft.EntityFrameworkCore;
using SeatReservationService.Consumers;
using SeatReservationService.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<DBIntializer>();

builder.Services.AddDbContext<ReservationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SeatReservationConsumer>();
    //x.AddConsumer<PaymentFailedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://host.docker.internal"), hst =>
        {
            hst.Username("admin");
            hst.Password("admin");
            cfg.ConfigureEndpoints(context);
        });
    });
});

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
