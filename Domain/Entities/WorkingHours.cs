namespace Domain.Entities;

public class WorkingHours
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid? RoomId { get; set; }
    public int Weekday { get; set; } // 0..6
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}