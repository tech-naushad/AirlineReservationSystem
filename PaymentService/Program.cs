using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Consumers;
using PaymentService.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<DBIntializer>();

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentConsumer>();
    x.AddConsumer<PaymentFailConsumer>();

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
