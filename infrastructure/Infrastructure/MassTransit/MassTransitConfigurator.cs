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
            params Type[] consumers
        )
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumers(consumers); // Registers all provided consumers
                x.UsingRabbitMq((context, cfg) => ConfigureRabbitMq(cfg, context, configuration));
            });
        }

        public static void ConfigureMassTransit<TSaga, TState, TSagaDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringKey,
            params Type[] consumers
        ) where TSaga : class, SagaStateMachine<TState>
          where TState : class, SagaStateMachineInstance
          where TSagaDbContext : DbContext
        {
            var dbConnectionString = configuration.GetConnectionString(connectionStringKey);

            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<TSaga, TState>()
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                        r.AddDbContext<DbContext, TSagaDbContext>((provider, options) =>
                            options.UseSqlServer(dbConnectionString));
                    });

                x.AddConsumers(consumers);
                x.UsingRabbitMq((context, cfg) => ConfigureRabbitMq(cfg, context, configuration));
            });
        }

        private static void ConfigureRabbitMq(IRabbitMqBusFactoryConfigurator cfg, IBusRegistrationContext context, IConfiguration configuration)
        {
            var rabbitMqSettings = configuration.GetSection("RabbitMQ");
            cfg.Host(new Uri(rabbitMqSettings["Host"]!), hst =>
            {
                hst.Username(rabbitMqSettings["Username"]!);
                hst.Password(rabbitMqSettings["Password"]!);
            });
            cfg.ConfigureEndpoints(context);
        }
    }
}