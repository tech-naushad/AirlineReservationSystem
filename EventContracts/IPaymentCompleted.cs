namespace EventContracts
{
    public interface IPaymentCompleted
    {
        Guid BookingId { get; init; }
        Guid PaymentId { get; set; }
    }
}
