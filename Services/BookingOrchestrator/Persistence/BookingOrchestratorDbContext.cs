using BookingOrchestrator.Orchestrator;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BookingOrchestrator.Persistence
{
    public class BookingOrchestratorDbContext : SagaDbContext
    {
        public BookingOrchestratorDbContext(DbContextOptions<BookingOrchestratorDbContext> options) : base(options)
        {

        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new BookingStateMap();
            }
        }
    }
    public class BookingStateMap : SagaClassMap<BookingState>
    {
        protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder modelBuilder)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(30);

            modelBuilder.Entity<BookingState>()
              .Property(b => b.Amount)
              .HasColumnType("decimal(18,2)");
        }
    }
}
