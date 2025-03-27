namespace PaymentService.Persistence.Entity
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }       
        public DateTime PaymentDate { get; set; }        
    }
}
