namespace SharedKernel.Events
{
    public interface IPaymentCompletedEvent
    {
        Guid BookingId { get; }
    }
}
