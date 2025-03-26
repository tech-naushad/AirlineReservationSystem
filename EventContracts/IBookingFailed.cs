namespace EventContracts
{
    public interface IBookingFailed: IBookingCreating
    {
        Guid TransactionId { get; }
        string BookingRequest { get; set; }
        string Reason { get; set; }      
    }
}
