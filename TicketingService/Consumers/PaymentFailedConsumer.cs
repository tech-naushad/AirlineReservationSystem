using EventContracts;
using MassTransit;
using MessageContracts;

namespace PaymentService.Consumers
{
    //public class TicketFailedConsumer : IConsumer<tick>
    //{
    //    private readonly ILogger<PaymentFailedConsumer> _logger;
    //    private readonly IPublishEndpoint _publishEndpoint;
    //    //private readonly PaymentDbContext _context;

    //    public PaymentFailedConsumer(ILogger<PaymentFailedConsumer> logger,
    //        IPublishEndpoint publishEndpoint)
    //    {
    //        _logger = logger;
    //        _publishEndpoint = publishEndpoint;
    //        //_context = context;

    //    }

    //    public async Task Consume(ConsumeContext<PaymentFailedContract> context)
    //    {
    //        _logger.LogInformation($"Processing payment for bookingId: {context.Message.BookingId}", context.Message.BookingNumber);

    //        // Simulate payment processing
    //       // await Task.Delay(1000);

    //        // 90% success rate
    //        //if (context.Message.Amount > 0)
    //        //{

    //            //using var transaction = await _context.Database.BeginTransactionAsync();

    //            //try
    //            //{
    //            //    var newPayment = new Payment
    //            //    {
    //            //        OrderId = context.Message.OrderId,
    //            //        Amount = context.Message.Amount,
    //            //        CreatedAt = DateTime.UtcNow,
    //            //        Status = "Processing"
    //            //    };

    //            //    await _context.Payments.AddAsync(newPayment);

    //            //    var result = await _context.SaveChangesAsync();

    //            //    if (result > 0)
    //            //    {
    //            //        await _publishEndpoint.Publish(new PaymentProcessed
    //            //        {
    //            //            OrderId = context.Message.OrderId,
    //            //            PaymentIntentId = newPayment.Id
    //            //        });
    //            //        newPayment.Status = "Success";
    //            //    }
    //            //    else
    //            //    {
    //            //        throw new Exception("Payment failed");
    //            //    }
    //            //    await _context.SaveChangesAsync();
    //            //    await transaction.CommitAsync();
    //            //}
    //            //catch (Exception ex)
    //            //{
    //            //    await transaction.RollbackAsync();
    //            //    await _publishEndpoint.Publish(new PaymentProcessFailed
    //            //    {
    //            //        OrderId = context.Message.OrderId,
    //            //        Reason = $"Payment failed, {ex.Message}"
    //            //    });
    //            //}

             
    //    }
    //}
}
