using Microsoft.EntityFrameworkCore;
using SeatReservationService.Persistence;

namespace SeatReservationService.Persistence
{
    public class DBIntializer
    {
        private readonly ReservationDbContext _context;

        public DBIntializer(ReservationDbContext context)
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
