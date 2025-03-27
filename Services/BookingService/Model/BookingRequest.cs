using System.Text.Json.Serialization;

namespace BookingService.Model
{
    public class BookingRequest
    {
        [JsonIgnore]
        public Guid BookingId { get; private set; } = Guid.NewGuid();
        [JsonIgnore]
        public string BookingNumber { get; private set; }
        public string Mobile { get; set; }
        public string Email { get; set; }     
        public decimal Amount { get; set; }
        public string AirLine { get; set; }
        public string FlightNumber { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public string SeatNumber { get; set; }
        public BookingRequest()
        {
            BookingNumber = $"{AirLine}-{FlightNumber}";
        }
    }
}
