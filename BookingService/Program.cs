using BookingService.Orchestrator;
using BookingService.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddScoped<BookingRepository, BookingRepository>();
builder.Services.AddScoped<DBIntializer>();
builder.Services.AddScoped<SagaDBIntializer>();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<BookingSagaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<BookingOrchestrator, BookingState>()
    .EntityFrameworkRepository(r =>
     {
         r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
         r.AddDbContext<DbContext, BookingSagaDbContext>((provider, options) =>
         {
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
         });
     });
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
    var sagaInitialiser = scope.ServiceProvider.GetRequiredService<SagaDBIntializer>();
    await initialiser.InitialiseAsync();
    await sagaInitialiser.InitialiseAsync();
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
