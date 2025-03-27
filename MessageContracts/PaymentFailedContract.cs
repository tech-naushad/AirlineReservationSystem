namespace MessageContracts
{
    public record PaymentFailedContract
    {
        public Guid BookingId { get; init; }      
        public string Reason { get; init; }
    }
}
