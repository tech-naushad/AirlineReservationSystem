
namespace SharedKernel.Messages
{
    public class PaymentContract
    {
        public Guid BookingId { get; init; }
        public decimal Amount { get; init; }
    }
}
