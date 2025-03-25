using EventContracts;
using MassTransit;
using MessageContracts;

namespace BookingService.Orchestrator
{
    public class BookingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid BookingId { get; set; }
        public string BookingNumber { get; set; }
        public decimal Amount {  get; set; }
        public string CurrentState { get; set; }       
        public DateTime BookingDate { get; set; }       
    }
    public class BookingOrchestrator : MassTransitStateMachine<BookingState>
    {
        public State ProcessingPayment { get; set; }
        public State PaymentFailed { get; set; }

        public Event<BookingCreated> OnBookingCreated { get; set; }
        public Event<PaymentCompleted> OnPaymentCompleted { get; set; }
        public Event<PaymentFailed> OnPaymentFailed { get; set; }


        public BookingOrchestrator()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OnBookingCreated, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => OnPaymentCompleted, x => x.CorrelateById(m => m.Message.BookingId));
            Event(() => OnPaymentFailed, x => x.CorrelateById(m => m.Message.BookingId));

            Initially
            (
                When(OnBookingCreated).Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                    context.Saga.BookingNumber = context.Message.BookingNumber;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.BookingDate = DateTime.UtcNow;
                })
                .PublishAsync(context => context.Init<PaymentContract>(new
                {
                    BookingId = context.Message.BookingId,
                    BookingNumber = context.Message.BookingNumber,
                    Amount = context.Message.Amount,
                })).TransitionTo(ProcessingPayment)
            );

            During(ProcessingPayment,
                When(OnPaymentCompleted).Then(context =>
                {
                    context.Saga.BookingId = context.Message.BookingId;
                    context.Saga.BookingNumber = context.Message.BookingNumber;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.BookingDate = DateTime.UtcNow;
                })
                //.TransitionTo(ReservingInventory)
                );

            SetCompletedWhenFinalized();
        }
    }
}
