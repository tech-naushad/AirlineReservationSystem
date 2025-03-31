using Microsoft.EntityFrameworkCore;

namespace BookingService.Persistence
{
    public class DBIntializer
    {
        private readonly BookingStateDbContext _context;

        public DBIntializer(BookingStateDbContext context)
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
