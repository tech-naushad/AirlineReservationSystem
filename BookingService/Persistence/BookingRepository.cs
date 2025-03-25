using BookingService.Persistence.Entity;
using EventContracts;
using MassTransit;

namespace BookingService.Persistence
{
    public interface IBookingRepository
    {
        public Task<Booking> CreateAsync(Booking booking);
    }
    public class BookingRepository: IBookingRepository
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly BookingDbContext _dbContext;
        public BookingRepository(BookingDbContext dbContext, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            //booking.DateCreated = DateTime.UtcNow;

            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();

            await _publishEndpoint.Publish(new BookingCreated
            {
                BookingId = booking.Id,     
                BookingNumber = booking.BookingNumber,
                Amount = booking.Amount
            });
            return booking;
        }
    }
}
