using Microsoft.EntityFrameworkCore;
using PaymentService.Persistence.Entity;

namespace PaymentService.Persistence
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentFailure> PaymentFailures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Payment>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasMaxLength(50);

            modelBuilder.Entity<PaymentFailure>()
              .HasKey(p => p.Id);
        }
    }
}
