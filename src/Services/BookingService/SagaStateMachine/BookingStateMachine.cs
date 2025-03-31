using BookingService.Model;
using SharedKernel.Events;
using MassTransit;
using SharedKernel.Messages;

namespace BookingService.SagaStateMachine
{
    public class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State ProcessingPayment { get; }
        public State ProcessingSeatReservation { get; }
        public State ProcessingBookingConfirmation { get; }
        //public State BookingSucccessful { get; }
        //public State BookingFailed { get; }                    
        public State BookingCompleted { get; }

        public Event<BookingRequest> BookingInitiated { get; private set; }
        public Event<IPaymentCompletedEvent> PaymentCompleted { get; private set; }
        public Event<ISeatReservationCompletedEvent> ReservationCompleted { get; private set; }
        public Event<IBookingConfirmedEvent> BookingConfirmed { get; private set; }

        public BookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => BookingInitiated, x => x.CorrelateById(m => m.Message.BookingId));
            //Event(() => OnBookingFailed, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => PaymentCompleted, x => x.CorrelateById(m => m.Message.BookingId));
            // Event(() => OnPaymentFailed, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => ReservationCompleted, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => BookingConfirmed, x => x.CorrelateById(m => m.Message.BookingId));

            Initially
            (
                When(BookingInitiated).Then(context =>
                {
                    var booking = context.Message;
                    var bookingNumber = $"{booking.AirLine}-{booking.FlightNumber}-{GenerateRandomAlphanumeric(3)}";

                    context.Saga.BookingNumber = bookingNumber;
                    context.Saga.BookingDate = DateTime.UtcNow;
                    context.Saga.Mobile = booking.Mobile;
                    context.Saga.Email = booking.Email;
                    context.Saga.Amount = booking.Amount;
                    context.Saga.AirLine = booking.AirLine;
                    context.Saga.FlightNumber = booking.FlightNumber;
                    context.Saga.Departure = booking.Departure;
                    context.Saga.Destination = booking.Destination;
                    context.Saga.DepartureTime = booking.DepartureTime;
                    context.Saga.SeatNumber = booking.SeatNumber;
                })
                .PublishAsync(context => context.Init<PaymentContract>(new
                {
                    BookingId = context.Saga.CorrelationId,
                    Amount = context.Saga.Amount
                }))
                .TransitionTo(ProcessingPayment)           
            );

            During(ProcessingPayment,
                When(PaymentCompleted)
                .PublishAsync(context => context.Init<SeatReservationContract>(new
                {
                    context.Message.BookingId,
                    context.Saga.FlightNumber

                })).TransitionTo(ProcessingSeatReservation)             
             );

            During(ProcessingSeatReservation,
                 When(ReservationCompleted)
                 .Then(context => Console.WriteLine($"Publishing BookingConfirmedContract for BookingId: {context.Saga.CorrelationId}"))
                 .PublishAsync(context => context.Init<BookingConfirmedContract>(new
                 {
                     BookingId = context.Saga.CorrelationId
                 }))
                 .TransitionTo(ProcessingBookingConfirmation)                 
                );
            
            During(ProcessingBookingConfirmation,
                When(BookingConfirmed)                 
                .TransitionTo(BookingCompleted)
               );

            SetCompletedWhenFinalized();
        }
        private static string GenerateRandomAlphanumeric(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }
    }
}
