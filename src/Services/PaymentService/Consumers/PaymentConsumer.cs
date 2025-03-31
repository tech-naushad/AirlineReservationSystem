
using SharedKernel.Events;
using MassTransit; 
using SharedKernel.Messages;
using Microsoft.EntityFrameworkCore;
using PaymentService.Persistence;
using PaymentService.Persistence.Entity;

namespace PaymentService.Consumers
{
    public class PaymentConsumer : IConsumer<PaymentContract>
    {
        private readonly ILogger<PaymentConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly PaymentDbContext _context;

        public PaymentConsumer(ILogger<PaymentConsumer> logger,
            IPublishEndpoint publishEndpoint, PaymentDbContext context)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _context = context;
        }

        public async Task Consume(ConsumeContext<PaymentContract> context)
        {
            _logger.LogInformation($"Processing payment for bookingId: {context.Message.BookingId}");
                        
            if (context.Message.Amount > 0)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var newPayment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        BookingId = context.Message.BookingId,
                        PaymentDate = DateTime.UtcNow
                    };
                    await _context.Payments.AddAsync(newPayment);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    await _publishEndpoint.Publish<IPaymentCompletedEvent>(new
                    {
                        context.Message.BookingId
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    //await _publishEndpoint.Publish<IPaymentFailed>(new
                    //{
                    //    BookingId = payment.BookingId,
                    //    Reason = $"Payment failed, {ex.Message}"
                    //});
                }                
            }
            else
            {
                //await _publishEndpoint.Publish<IPaymentFailed>(new
                //{
                //    BookingId = context.Message.BookingId,              
                //    Reason = "Payment failed as the booking amount is less than zero"
                //});
            }
        }
    }
}
