using EventContracts;
using MassTransit;
using PaymentService.Persistence.Entity;

namespace PaymentService.Persistence
{
    public interface IPaymentRepository
    {
        Task<Payment> ProcessPaymentAsync(Payment payment);
        Task<PaymentFailure> ProcessFailedPaymentAsync(PaymentFailure paymentFailure);
         
    }
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly PaymentDbContext _context;
        public PaymentRepository(PaymentDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }        
        public async Task<Payment> ProcessPaymentAsync(Payment payment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                payment.Status = "Completed";
                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                await _publishEndpoint.Publish<IPaymentFailed>(new
                {
                    BookingId = payment.BookingId,                  
                    Reason = $"Payment failed, {ex.Message}"
                });
            }
            return payment;
        }

        public async Task<PaymentFailure> ProcessFailedPaymentAsync(PaymentFailure paymentFailure)
        {
            await _context.PaymentFailures.AddAsync(paymentFailure);
            await _context.SaveChangesAsync();
            return paymentFailure;
        }
    }
}
