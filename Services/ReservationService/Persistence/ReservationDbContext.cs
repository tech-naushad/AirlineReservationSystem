using Microsoft.EntityFrameworkCore;
using PaymentService.Persistence.Entity;

namespace SeatReservationService.Persistence
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions<ReservationDbContext> options) : base(options) { }
        public DbSet<SeatReservation> SeatReservations { get; set; }
        //public DbSet<PaymentFailure> PaymentFailures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SeatReservation>()
                .HasKey(p => p.Id);           

            //modelBuilder.Entity<PaymentFailure>()
            //  .HasKey(p => p.Id);
        }
    }
}
