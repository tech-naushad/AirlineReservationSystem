using EventContracts;
using MassTransit;
using MessageContracts;

namespace PaymentService.Consumers
{
    public class TicketConsumer : IConsumer<TicketContract>
    {
        private readonly ILogger<TicketConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        //private readonly IPaymentRepository _repository;
        

        public TicketConsumer(ILogger<TicketConsumer> logger,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;          
           //_repository = repository;
        }

        public async Task Consume(ConsumeContext<TicketContract> context)
        {
            _logger.LogInformation($"Processing ticket for bookingId: {context.Message.BookingId}", context.Message.BookingNumber);

            var ticketNumber = $"TCK-{context.Message.BookingNumber}-{GenerateRandomAlphanumeric(4)}";
            // 90% success rate
            //if (context.Message.Amount > 0)
            //{
            //    await _repository.ProcessPaymentAsync(new Payment 
            //    { 
            //        BookingId = context.Message.BookingId, 
            //        BookingNumber = context.Message.BookingNumber,
            //        Amount = context.Message.Amount,
            //    });
            //}
            //else
            //{
            //    await _publishEndpoint.Publish<IPaymentFailed>(new
            //    {
            //        BookingId = context.Message.BookingId,
            //        BookingNumber = context.Message.BookingNumber,
            //        Reason = "Payment failed as the booking amount is less than zero"
            //    });
            //}
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
