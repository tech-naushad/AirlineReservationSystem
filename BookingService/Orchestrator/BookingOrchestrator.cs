using BookingService.Models;
using EventContracts;
using MassTransit;
using MessageContracts;

namespace BookingService.Orchestrator
{
    public class BookingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid BookingId { get; set; }       
        public string CurrentState { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class BookingOrchestrator : MassTransitStateMachine<BookingState>
    {
       // public State BookingCreating { get; set; }
        public State BookingCreated { get; set; }
        public State BookingFailed { get; set; }
        public State ProcessingPayment { get; set; }
        public State PaymentFailed { get; set; }
        public State ProcessingTicket { get; set; }
        public State ProcessingTicketFailed { get; set; }
        public State ProcessingNotification { get; set; }       
        public State Completed { get; private set; }

        //public Event<IBookingCreating> OnBookingCreating { get; set; }
        public Event<IBookingCreated> OnBookingCreated { get; set; }
        public Event<IBookingFailed> OnBookingFailed { get; set; }
       
        // public Event<IPaymentCompleted> OnPaymentCompleted { get; set; }
      //  public Event<IPaymentFailed> OnPaymentFailed { get; set; }
       // public Event<ITicketProcessed> OnTicketProcessed { get; set; }

        public BookingOrchestrator()
        {
            InstanceState(x => x.CurrentState);

            //Event(() => obboo, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => OnBookingCreated, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => OnBookingFailed, x => x.CorrelateById(m => m.Message.TransactionId));
            // Event(() => OnPaymentCompleted, x => x.CorrelateById(m => m.Message.BookingId));
            //  Event(() => OnPaymentFailed, x => x.CorrelateById(m => m.Message.BookingId));
            //Event(() => OnTicketProcessed, x => x.CorrelateById(m => m.Message.BookingId));          

            Initially
            (
                When(OnBookingCreated).Then(context =>
                {
                    context.Saga.BookingId = Guid.NewGuid();
                    context.Saga.DateCreated = DateTime.UtcNow;
                })
                .PublishAsync(context => context.Init<PaymentContract>(context))
                .TransitionTo(ProcessingPayment),

                When(OnBookingFailed).Then(context =>
                {
                     
                    context.Saga.DateCreated = DateTime.UtcNow;
                })
                .PublishAsync(context => context.Init<BookingFailedContract>(new
                {
                    context.Message.TransactionId,
                    context.Message.BookingRequest,
                    context.Message.Reason

                }))
                .TransitionTo(BookingFailed)
                 .Finalize()
            );

            //During(ProcessingPayment,
            //    When(OnPaymentCompleted).Then(context =>
            //    {
            //        context.Saga.BookingId = context.Message.BookingId;
            //        //context.Saga.BookingNumber = context.Message.BookingNumber;
            //        //context.Saga.Amount = context.Message.Amount;
            //    })
            //    .PublishAsync(context => context.Init<TicketContract>(new
            //    {
            //        context.Message.BookingId                    ,
            //        //context.Message.BookingNumber
            //    })).TransitionTo(ProcessingTicket)
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
