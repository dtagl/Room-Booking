namespace Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid? ActorUserId { get; set; }
    public string Action { get; set; } = default!;
    public string TargetType { get; set; } = default!;
    public string TargetId { get; set; } = default!;
    public string? Before { get; set; }
    public string? After { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}