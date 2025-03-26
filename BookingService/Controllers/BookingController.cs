using BookingService.Models;
using BookingService.Persistence;
using BookingService.Persistence.Entity;
using EventContracts;
using MassTransit;
using MessageContracts;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBookingRepository _repository;

        public BookingController(ILogger<BookingController> logger,
            IBookingRepository repository)
        {
            _logger = logger;           
            _repository = repository;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            if (request == null)
                return BadRequest("Invalid booking request.");

            var booking = new Booking
            {
                Email = request.Email,
                Mobile = request.Mobile,
                FlightNumber = request.FlightNumber,               
                DepartureDate = request.DepartureDate,
                Amount = request.Amount,
                AirlineCode = request.AirlineCode                
            };

            var response = await _repository.CreateAsync(booking);

            return Ok(new { Message = $"Your booking has been placed. Your booking reference is {booking.BookingNumber}" +
                $"You will get your itenary details soon" });
        } 
    }
}
