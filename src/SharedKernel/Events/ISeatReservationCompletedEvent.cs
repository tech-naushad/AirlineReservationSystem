
namespace SharedKernel.Events
{
    public interface ISeatReservationCompletedEvent
    {
        Guid BookingId { get; }
    }
}
