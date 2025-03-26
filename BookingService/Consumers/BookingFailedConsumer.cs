using BookingService.Persistence;
using BookingService.Persistence.Entity;
using MassTransit;
using MessageContracts;

namespace BookingService.Consumers
{
    public class BookingFailedConsumer : IConsumer<BookingFailedContract>
    {
        private readonly ILogger<BookingFailedConsumer> _logger;
        private readonly IBookingRepository _repository;
        public BookingFailedConsumer(ILogger<BookingFailedConsumer> logger, IBookingRepository repository)
        {
            _logger = logger; 
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<BookingFailedContract> context)
        {
            var transaction = new BookingTransaction
            {
                TransactionId = context.Message.TransactionId,
                Request = context.Message.BookingRequest,
                Reason = context.Message.Reason,
                DateCreated = DateTime.UtcNow
            };
           await _repository.LogTransactionAsync(transaction);
        }
    }
}
