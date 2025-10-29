namespace Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Room Room { get; set; } = default!;
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CanceledAt { get; set; }
    public Guid? CanceledBy { get; set; }
    public string? Meta { get; set; }
}