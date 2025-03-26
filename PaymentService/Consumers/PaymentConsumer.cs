using EventContracts;
using MassTransit;
using MessageContracts;
using PaymentService.Persistence;

namespace PaymentService.Consumers
{
    public class PaymentConsumer : IConsumer<PaymentContract>
    {
        private readonly ILogger<PaymentConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IPaymentRepository _repository;
        

        public PaymentConsumer(ILogger<PaymentConsumer> logger,
            IPublishEndpoint publishEndpoint,IPaymentRepository repository)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;          
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<PaymentContract> context)
        {
            _logger.LogInformation($"Processing payment for bookingId: {context.Message.BookingId}", context.Message.BookingNumber);

            // 90% success rate
            if (context.Message.Amount > 0)
            {
                await _repository.ProcessPaymentAsync(new Payment 
                { 
                    BookingId = context.Message.BookingId, 
                    BookingNumber = context.Message.BookingNumber,
                    Amount = context.Message.Amount,
                });
            }
            else
            {
                await _publishEndpoint.Publish<IPaymentFailed>(new
                {
                    BookingId = context.Message.BookingId,
                    BookingNumber = context.Message.BookingNumber,
                    Reason = "Payment failed as the booking amount is less than zero"
                });
            }
        }
    }
}
