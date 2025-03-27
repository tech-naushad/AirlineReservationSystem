using MassTransit;

namespace BookingOrchestrator.Orchestrator
{
    public class BookingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; } // Required for MassTransit Saga
        public Guid BookingId { get; set; }
        public string BookingNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }        
        public decimal Amount { get; set; }
        public string AirLine { get; set; }
        public string FlightNumber { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public string SeatNumber { get; set; }
        public string CurrentState { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } 
    }
}
