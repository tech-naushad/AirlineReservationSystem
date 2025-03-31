namespace PaymentService.Persistence.Entity
{
    public class SeatReservation
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }       
        public DateTime ReservationDate { get; set; }        
    }
}
