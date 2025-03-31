using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Consumers;
using PaymentService.Persistence;
using Infrastructure.MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<DBIntializer>();

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDatabase")));

// Configure MassTransit and register multiple consumers
builder.Services.ConfigureMassTransit(builder.Configuration,
    typeof(PaymentConsumer)
//typeof(PaymentFailedConsumer) // Add more consumers here
);

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
