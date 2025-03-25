using BookingService.Models;
using BookingService.Persistence;
using BookingService.Persistence.Entity;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        //private readonly IPublishEndpoint _publishEndpoint;
        private readonly BookingRepository _repository;

        public BookingController(ILogger<BookingController> logger, 
            BookingRepository repository)
        {
            _logger = logger;
            //_publishEndpoint = publishEndpoint;
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
                AirlineCode = request.AirlineCode,                
                BookingNumber = $"{request.AirlineCode}-{request.FlightNumber}-{GenerateRandomAlphanumeric(8)}"
            };

            var response = await _repository.CreateAsync(booking);
            
            return Ok(new { Message = $"Your booking has been placed with booking reference number {response.BookingNumber}. " +
                $"You will get the itenary details soon" });
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
