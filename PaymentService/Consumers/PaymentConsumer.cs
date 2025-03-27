using EventContracts;
using MassTransit;
using MessageContracts;
using PaymentService.Persistence;
using PaymentService.Persistence.Entity;

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
            _logger.LogInformation($"Processing payment for bookingId: {context.Message.BookingId}");
                        
            if (context.Message.Amount > 0)
            {
                var paymentId = Guid.NewGuid();
                await _repository.ProcessPaymentAsync(new Payment 
                { 
                    Id = paymentId,
                    BookingId = context.Message.BookingId 
                });

                await _publishEndpoint.Publish<IPaymentCompleted>(new
                {
                    BookingId = context.Message.BookingId,
                    PaymentId = paymentId
                });
            }
            else
            {
                await _publishEndpoint.Publish<IPaymentFailed>(new
                {
                    BookingId = context.Message.BookingId,              
                    Reason = "Payment failed as the booking amount is less than zero"
                });
            }
        }
    }
}
