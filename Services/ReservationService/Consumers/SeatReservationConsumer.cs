using Events;
using MassTransit;
using Messages;
using PaymentService.Persistence.Entity;
using SeatReservationService.Persistence;

namespace SeatReservationService.Consumers
{
    public class SeatReservationConsumer : IConsumer<SeatReservationContract>
    {
        private readonly ILogger<SeatReservationConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ReservationDbContext _context;

        public SeatReservationConsumer(ILogger<SeatReservationConsumer> logger
            , ReservationDbContext context,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _context = context;
        }

        public async Task Consume(ConsumeContext<SeatReservationContract> context)
        {
            _logger.LogInformation($"Processing Seat Reservation for bookingId: {context.Message.BookingId}");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var newReservation = new SeatReservation
                {
                    Id = Guid.NewGuid(),
                    BookingId = context.Message.BookingId,
                    ReservationDate = DateTime.UtcNow
                };
                await _context.SeatReservations.AddAsync(newReservation);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await _publishEndpoint.Publish<ISeatReservationCompletedEvent>(new
                {
                    context.Message.BookingId
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                //await _publishEndpoint.Publish<IPaymentFailed>(new
                //{
                //    BookingId = payment.BookingId,
                //    Reason = $"Payment failed, {ex.Message}"
                //});
            }


        }
    }
}
