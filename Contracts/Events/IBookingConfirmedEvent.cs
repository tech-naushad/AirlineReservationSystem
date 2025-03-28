namespace Events
{
    public interface IBookingConfirmedEvent
    {
        Guid BookingId { get; set;}
    }
}
