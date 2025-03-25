using Microsoft.EntityFrameworkCore;

namespace PaymentService.Persistence
{
    public class DBIntializer
    {
        private readonly PaymentDbContext _context;

        public DBIntializer(PaymentDbContext context)
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
