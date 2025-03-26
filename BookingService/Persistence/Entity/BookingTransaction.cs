namespace BookingService.Persistence.Entity
{
    public class BookingTransaction
    {
        public Guid TransactionId { get; set; }
        public string Request { get; set; }
        public string Reason { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
