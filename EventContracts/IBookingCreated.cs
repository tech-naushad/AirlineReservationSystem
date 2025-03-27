namespace EventContracts
{
    public interface IBookingCreated
    {
        Guid BookingId { get; set; }     
        decimal Amount { get; set; }
    }
}
