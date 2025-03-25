using Microsoft.EntityFrameworkCore;

namespace BookingService.Persistence
{
    public class DBIntializer
    {
        private readonly BookingDbContext _context;

        public DBIntializer(BookingDbContext context)
        {
            _context = context;
        }
        public async Task InitialiseAsync()
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
    }
    public class SagaDBIntializer
    {
        private readonly BookingSagaDbContext _context;

        public SagaDBIntializer(BookingSagaDbContext context)
        {
            _context = context;
        }
        public async Task InitialiseAsync()
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
    }
}
