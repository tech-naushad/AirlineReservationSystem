namespace PaymentService.Persistence.Entity
{
    public class PaymentFailure
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; } 
        public string Reason { get; set; } 
        public DateTime CreatedDate { get; set; }

        public PaymentFailure()
        {   
            CreatedDate = DateTime.UtcNow;
        }
    }
}
