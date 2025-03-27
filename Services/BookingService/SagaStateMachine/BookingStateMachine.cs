using BookingService.Model;
using Events;
using MassTransit;
using Messages;
using System.Reflection;

namespace BookingService.SagaStateMachine
{
    public class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        public State ProcessingPayment{ get; private set; }
        public State ProcessingSeatReservation { get; private set; }
        public State BookingConfirmed { get; private set; }
        public State BookingFailed { get; private set; }
        //public State BookingCreated { get; set; }
        //public State BookingFailed { get; set; }
        //public State ProcessingPayment { get; set; }
        //public State PaymentFailed { get; set; }
        //public State ProcessingTicket { get; set; }
        //public State ProcessingTicketFailed { get; set; }             
        //public State Completed { get; private set; }

        public Event<BookingRequest> BookingInitiated { get; set; }
        public Event<IPaymentCompletedEvent> PaymentCompleted  { get; private set; }
        //public Event<IBookingFailed> OnBookingFailed { get; set; }       
        //public Event<IPaymentCompleted> OnPaymentCompleted { get; set; }
        //public Event<IPaymentFailed> OnPaymentFailed { get; set; }    

        public BookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => BookingInitiated, x => x.CorrelateById(m => m.Message.BookingId));
            //Event(() => OnBookingFailed, x => x.CorrelateById(m => m.Message.TransactionId));
            Event(() => PaymentCompleted, x => x.CorrelateById(m => m.Message.BookingId));
            // Event(() => OnPaymentFailed, x => x.CorrelateById(m => m.Message.BookingId));


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

            //When(OnBookingFailed).Then(context =>
            //{                     
            //    context.Saga.DateCreated = DateTime.UtcNow;
            //})
            //.PublishAsync(context => context.Init<BookingFailedContract>(new
            //{
            //    context.Message.TransactionId,
            //    context.Message.BookingRequest,
            //    context.Message.Reason

            //}))
            //.TransitionTo(BookingFailed)
            // .Finalize()
            );

            During(ProcessingPayment,
                When(PaymentCompleted)
                .PublishAsync(context => context.Init<ReserveSeatContract>(new
                {
                    context.Message.BookingId,
                    context.Saga.FlightNumber

                })).TransitionTo(ProcessingSeatReservation)

                //When(OnPaymentFailed).Then(context =>
                //{
                //    //context.Saga.DateCreated = DateTime.UtcNow;
                //})
                //.PublishAsync(context => context.Init<PaymentFailedContract>(new
                //{
                //    BookingId = context.Message.BookingId,
                //    Reason = context.Message.Reason
                //}))
                //.TransitionTo(BookingFailed)
                // .Finalize()
             );

            //During(ProcessingTicket,
            //     When(OnTicketProcessed).Then(context =>
            //     {
            //         context.Saga.BookingId = context.Message.BookingId;
            //         //context.Saga.BookingDate = DateTime.UtcNow;
            //     })
            //     //.PublishAsync(context => context.Init<TicketContract>(new
            //     //{
            //     //    context.Message.BookingId
            //     //}))
            //     .TransitionTo(ProcessingNotification)
            //     );

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
