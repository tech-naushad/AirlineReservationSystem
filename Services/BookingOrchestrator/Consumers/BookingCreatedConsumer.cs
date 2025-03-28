﻿using BookingOrchestrator.Orchestrator;
using Events;
using MassTransit;

namespace BookingOrchestrator.Consumers
{
    public class BookingCreatedConsumer : IConsumer<IBookingCreatedEvent>
    {
        private readonly ILogger<BookingCreatedConsumer> _logger;

        public BookingCreatedConsumer(ILogger<BookingCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IBookingCreatedEvent> context)
        {
            var booking = context.Message;
            //_logger.LogInformation($"Received BookingCreatedEvent: {booking.BookingId}");

            // Start Saga
            await context.Publish(new BookingState
            {
                BookingId = booking.BookingId,
                BookingNumber = booking.BookingNumber,
                BookingDate = booking.BookingDate,
                Email = booking.Email,
                Mobile = booking.Mobile,
                Amount = booking.Amount,
                AirLine = context.Message.AirLine,
                FlightNumber = booking.FlightNumber,
                Departure = booking.Departure,
                Destination = booking.Destination,
                DepartureTime = booking.DepartureTime,
                SeatNumber = booking.SeatNumber
            });
        }
    }
}
