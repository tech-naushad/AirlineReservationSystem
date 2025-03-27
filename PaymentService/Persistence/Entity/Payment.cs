namespace PaymentService.Persistence.Entity
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; } 
        public string Status { get; set; } 
        public DateTime CreatedDate { get; set; }

        public Payment()
        {           
            Status = "Pending";
            CreatedDate = DateTime.UtcNow;
        }
    }
}
