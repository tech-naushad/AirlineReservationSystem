using Microsoft.EntityFrameworkCore;

namespace BookingOrchestrator.Persistence
{
    public class DBIntializer
    {
        private readonly BookingOrchestratorDbContext _context;

        public DBIntializer(BookingOrchestratorDbContext context)
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
