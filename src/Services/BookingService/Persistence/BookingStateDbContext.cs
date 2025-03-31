using BookingService.SagaStateMachine;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Persistence
{
    public class BookingStateDbContext : SagaDbContext
    {
        public BookingStateDbContext(DbContextOptions<BookingStateDbContext> options) : base(options)
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
