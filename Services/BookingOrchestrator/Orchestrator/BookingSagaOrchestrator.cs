using Events;
using MassTransit;
using Messages;

namespace BookingOrchestrator.Orchestrator
{
    public class BookingSagaOrchestrator : MassTransitStateMachine<BookingState>
    {
        public State BookingCreated { get; set; }
        public State BookingFailed { get; set; }
        public State ProcessingPayment { get; set; }
        //public State PaymentFailed { get; set; }
        //public State ProcessingTicket { get; set; }
        //public State ProcessingTicketFailed { get; set; }             
        public State Completed { get; private set; }
  
        public Event<IBookingCreatedEvent> BookingCreatedEvent { get; set; }
        //public Event<IBookingFailed> OnBookingFailed { get; set; }       
        //public Event<IPaymentCompleted> OnPaymentCompleted { get; set; }
        //public Event<IPaymentFailed> OnPaymentFailed { get; set; }    

        public BookingSagaOrchestrator()
        {
            InstanceState(x => x.CurrentState);
            
            Event(() => BookingCreatedEvent, x => x.CorrelateById(m => m.Message.BookingId));
            //Event(() => OnBookingFailed, x => x.CorrelateById(m => m.Message.TransactionId));
            //Event(() => OnPaymentCompleted, x => x.CorrelateById(m => m.Message.BookingId));
           // Event(() => OnPaymentFailed, x => x.CorrelateById(m => m.Message.BookingId));
          

            Initially
            (
                When(BookingCreatedEvent).Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;                    
                })
                .PublishAsync(context => context.Init<PaymentContract>(new
                {
                    context.Message.BookingId,
                    //context.Message.BookingRequest.Amount,
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

            //During(ProcessingPayment,
            //    When(OnPaymentCompleted).Then(context =>
            //    {
            //        context.Saga.BookingId = context.Message.BookingId;
                    
            //    })
            //    .PublishAsync(context => context.Init<TicketContract>(new
            //    {
            //        context.Message.BookingId, 
                     
            //    })).TransitionTo(ProcessingTicket),

            //    When(OnPaymentFailed).Then(context =>
            //    {
            //        //context.Saga.DateCreated = DateTime.UtcNow;
            //    })
            //    .PublishAsync(context => context.Init<PaymentFailedContract>(new
            //    {
            //        BookingId = context.Message.BookingId,
            //        Reason = context.Message.Reason
            //    }))
            //    .TransitionTo(BookingFailed)
            //     .Finalize()
            // );

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
    }
}
