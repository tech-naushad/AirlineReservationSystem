using BookingService.Models;
using BookingService.Persistence.Entity;
using EventContracts;
using MassTransit;
using System.Text.Json;

namespace BookingService.Persistence
{
    public interface IBookingRepository
    {
        public Task<Booking> CreateAsync(Booking booking);
        public Task LogTransactionAsync(BookingTransaction transaction);
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
            booking.BookingNumber = $"{booking.AirlineCode}-{booking.FlightNumber}-{GenerateRandomAlphanumeric(6)}";
            await _dbContext.Bookings.AddAsync(booking);

            try
            {
                await _dbContext.SaveChangesAsync();
                
                await _publishEndpoint.Publish<IBookingCreated>(new
                {
                    BookingId = booking.Id,
                    Amount = booking.Amount,
                });
               
            }
            catch (Exception ex)
            {
                await _publishEndpoint.Publish<IBookingFailed>(new
                {
                    TransactionId = Guid.NewGuid(),
                    BookingRequest = JsonSerializer.Serialize(booking),
                    Reason = ex.Message
                });                
            }
            return booking;
        }
        public async Task LogTransactionAsync(BookingTransaction transaction)
        {
            await _dbContext.BookingTransactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
        }
        private static string GenerateRandomAlphanumeric(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }     
    }
}
