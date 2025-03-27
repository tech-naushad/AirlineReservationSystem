namespace MessageContracts
{
    public record PaymentContract
    {
        public Guid BookingId { get; init; } 
        public decimal Amount { get; init; }
    }
}
