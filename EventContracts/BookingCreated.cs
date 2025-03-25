namespace EventContracts
{
    public class BookingCreated
    {
        public Guid BookingId { get; set; }
        public string BookingNumber { get; set; }        
        public decimal Amount { get; set; }
    }
}
