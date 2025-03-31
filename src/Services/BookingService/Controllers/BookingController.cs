using BookingService.Model;
using BookingService.SagaStateMachine;
using SharedKernel.Events;
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
        private readonly IPublishEndpoint _publishEndpoint;

        public BookingController(ILogger<BookingController> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            if (request == null)
                return BadRequest("Invalid booking request.");
           
            await _publishEndpoint.Publish(request);

            return Ok(new { Message = $"Your booking has been placed." +
                $"You will get your itenary details soon" });
        }

        
    }
}
