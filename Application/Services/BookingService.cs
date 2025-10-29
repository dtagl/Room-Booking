using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class BookingService
{
    private readonly IUnitOfWork _uow;
    public BookingService(IUnitOfWork uow) { _uow = uow; }

    public async Task<Booking> CreateBookingAsync(Guid roomId, Guid userId, DateTime startAtUtc, DateTime endAtUtc, string? note = null)
    {
        if (endAtUtc <= startAtUtc) throw new ArgumentException("End must be after start");
        // Load room/company info
        var room = await _uow.Rooms.GetAsync(roomId) ?? throw new InvalidOperationException("Room not found");
        var companyId = room.CompanyId;

        // 1) Check double booking
        var conflicting = _uow.Bookings.Query()
            .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active &&
                        b.StartAt < endAtUtc && startAtUtc < b.EndAt)
            .Any();
        if (conflicting) throw new InvalidOperationException("Time slot is already taken");

        // 2) Optionally check working hours (skip here - can be added)

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            UserId = userId,
            CompanyId = companyId,
            StartAt = startAtUtc,
            EndAt = endAtUtc,
            Status = BookingStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Meta = note
        };
        await _uow.Bookings.AddAsync(booking);
        await _uow.SaveChangesAsync();
        return booking;
    }

    public async Task CancelBookingAsync(Guid bookingId, Guid actorUserId, bool actorIsAdmin)
    {
        var booking = await _uow.Bookings.GetAsync(bookingId) ?? throw new InvalidOperationException("Booking not found");
        if (!actorIsAdmin && booking.UserId != actorUserId) throw new UnauthorizedAccessException("No rights to cancel");
        booking.Status = BookingStatus.Cancelled;
        booking.CanceledAt = DateTime.UtcNow;
        booking.CanceledBy = actorUserId;
        _uow.Bookings.Update(booking);
        await _uow.SaveChangesAsync();
    }
}