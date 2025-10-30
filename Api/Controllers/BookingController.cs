using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("rooms")]
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;
    private readonly IUnitOfWork _uow;
    public BookingController(BookingService bookingService, IUnitOfWork uow) { _bookingService = bookingService; _uow = uow; }

    [HttpGet("{roomId}/availability")]
    public IActionResult GetAvailability(Guid roomId, [FromQuery] DateTime date) // date in UTC (YYYY-MM-DD)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);
        var bookings = _uow.Bookings.Query().Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active && b.StartAt < dayEnd && b.EndAt > dayStart).ToList();
        // Generate 15-min slots inside working hours — for MVP, assume 08:00..20:00 local UTC
        var slots = new List<object>();
        var workStart = dayStart.AddHours(8);
        var workEnd = dayStart.AddHours(20);
        for (var t = workStart; t < workEnd; t = t.AddMinutes(15))
        {
            var slotStart = t;
            var slotEnd = t.AddMinutes(15);
            var occupied = bookings.Any(b => b.StartAt < slotEnd && slotStart < b.EndAt);
            var bookingRef = occupied ? bookings.First(b => b.StartAt < slotEnd && slotStart < b.EndAt) : null;
            slots.Add(new { start = slotStart, end = slotEnd, isAvailable = !occupied, bookingId = bookingRef?.Id, bookedBy = bookingRef?.UserId });
        }
        return Ok(slots);
    }

    [Authorize]
    [HttpPost("{roomId}/booking")]
    public async Task<IActionResult> CreateBooking(Guid roomId, [FromBody] CreateBookingModel model)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "userId").Value);
        var booking = await _bookingService.CreateBookingAsync(roomId, userId, model.StartAt.ToUniversalTime(), model.EndAt.ToUniversalTime(), model.Note);
        return Ok(new { booking.Id, booking.StartAt, booking.EndAt });
    }

    [Authorize]
    [HttpDelete("booking/{id}")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "userId").Value);
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var isAdmin = role == "Admin";

        await _bookingService.CancelBookingAsync(id, userId, isAdmin);
        return Ok();
    }




    public record CreateBookingModel(DateTime StartAt, DateTime EndAt, string? Note);
}
