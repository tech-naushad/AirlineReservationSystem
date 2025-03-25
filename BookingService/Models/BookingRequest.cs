namespace BookingService.Models
{
    public class BookingRequest
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string AirlineCode { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal Amount { get; set; }     
        
    }
}
