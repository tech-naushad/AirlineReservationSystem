namespace PaymentService.Persistence
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public string BookingNumber { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } // "Pending", "Completed", "Failed"
        public DateTime CreatedDate { get; set; }

        public Payment()
        {
            Id= Guid.NewGuid();
            Status = "Pending";
            CreatedDate = DateTime.UtcNow;
        }
    }
}
