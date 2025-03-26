using BookingService.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Persistence
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingTransaction> BookingTransactions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Booking>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasMaxLength(50);

            modelBuilder.Entity<BookingTransaction>()
             .HasKey(b => b.TransactionId);
        }
    }
}
