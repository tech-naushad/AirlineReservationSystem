using BookingService.Orchestrator;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Persistence
{
    public class BookingSagaDbContext : SagaDbContext
    {
        public BookingSagaDbContext(DbContextOptions<BookingSagaDbContext> options) : base(options)
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
        protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(30);
            entity.Property(x => x.BookingNumber).HasMaxLength(30);             
        }
    }
}
