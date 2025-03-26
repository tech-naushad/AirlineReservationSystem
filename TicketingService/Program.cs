using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
//builder.Services.AddScoped<DBIntializer>();

//builder.Services.AddDbContext<PaymentDbContext>(options =>
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<TicketConsumer>();
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
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
