//using MassTransit;
//using MessageContracts;
//using PaymentService.Persistence;
//using PaymentService.Persistence.Entity;

//namespace PaymentService.Consumers
//{
//    public class PaymentFailedConsumer : IConsumer<PaymentFailedContract>
//    {
//        private readonly ILogger<PaymentFailedConsumer> _logger;
//        private readonly IPublishEndpoint _publishEndpoint;
//        private readonly IPaymentRepository _repository;
        
//        public PaymentFailedConsumer(ILogger<PaymentFailedConsumer> logger,
//            IPublishEndpoint publishEndpoint, IPaymentRepository repository)
//        {
//            _logger = logger;
//            _publishEndpoint = publishEndpoint; 
//            _repository = repository;
//        }

//        public async Task Consume(ConsumeContext<PaymentFailedContract> context)
//        {
//            _logger.LogInformation($"Processing failed payment for bookingId: {context.Message.BookingId}");

//            await _repository.ProcessFailedPaymentAsync(new PaymentFailure
//            {
//                Id = Guid.NewGuid(),
//                Reason = context.Message.Reason,
//                CreatedDate = DateTime.UtcNow
//            });
//        }
//    }
//}
