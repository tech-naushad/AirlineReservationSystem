using Events;
using MassTransit;
using Messages;

namespace BookingService.Consumers
{
    public class BookingConfirmedConsumer : IConsumer<BookingConfirmedContract>
    {
        private readonly ILogger<BookingConfirmedConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookingConfirmedConsumer(ILogger<BookingConfirmedConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<BookingConfirmedContract> context)
        {
            var booking = context.Message;
            _logger.LogInformation($"Received BookingCreatedEvent: {booking.BookingId}");

            await _publishEndpoint.Publish<IBookingConfirmedEvent>(new
            {
                context.Message.BookingId
            });
        }
    }
}
