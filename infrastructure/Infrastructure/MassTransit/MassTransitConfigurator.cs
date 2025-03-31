using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MassTransit
{
    public static class MassTransitConfigurator
    {
        public static void ConfigureMassTransit(
            this IServiceCollection services,
            IConfiguration configuration,
            params Type[] consumers // Accepts multiple consumers dynamically
        )
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMQ");         
            var rabbitMqHost = rabbitMqSettings["Host"];
            var rabbitMqUsername = rabbitMqSettings["Username"];
            var rabbitMqPassword = rabbitMqSettings["Password"];

            services.AddMassTransit(x =>
            {
                // Register all provided consumers
                foreach (var consumer in consumers)
                {
                    x.AddConsumer(consumer);
                }

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitMqHost), hst =>
                    {
                        hst.Username(rabbitMqUsername);
                        hst.Password(rabbitMqPassword);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }


        public static void ConfigureMassTransit<TSaga, TState, TSagaDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringKey,
            params Type[] consumers // Allows multiple consumers dynamically
        ) where TSaga : class, SagaStateMachine<TState>
          where TState : class, SagaStateMachineInstance
          where TSagaDbContext : DbContext
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMQ");
            var rabbitMqHost = rabbitMqSettings["Host"];
            var rabbitMqUsername = rabbitMqSettings["Username"];
            var rabbitMqPassword = rabbitMqSettings["Password"];
            var dbConnectionString = configuration.GetConnectionString(connectionStringKey);

            services.AddMassTransit(x =>
            {
                // Register Saga State Machine with Entity Framework
                x.AddSagaStateMachine<TSaga, TState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                        r.AddDbContext<DbContext, TSagaDbContext>((provider, options) =>
                        {
                            options.UseSqlServer(dbConnectionString);
                        });
                    });

                // Register multiple consumers dynamically
                foreach (var consumer in consumers)
                {
                    x.AddConsumer(consumer);
                }

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitMqHost), hst =>
                    {
                        hst.Username(rabbitMqUsername);
                        hst.Password(rabbitMqPassword);
                    });

                    cfg.ConfigureEndpoints(context);

                    // Uncomment to enable OpenTelemetry integration
                    // cfg.UseMessageData<OpenTelemetryMessageData>();
                });
            });
        }

    }
}
