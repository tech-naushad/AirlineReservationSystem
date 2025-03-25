namespace BookingService.Persistence.Entity
{
    public class Booking
    {
        public Guid Id { get; set; }
        public string BookingNumber { get; set; }
        public string AirlineCode { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Canceled
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Booking()
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
            Status = "Pending";
        }
    }
    
}
