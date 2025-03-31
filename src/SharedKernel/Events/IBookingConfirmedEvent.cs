namespace SharedKernel.Events
{
    public interface IBookingConfirmedEvent
    {
        Guid BookingId { get; set;}
    }
}
