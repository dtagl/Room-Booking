namespace Domain.Entities;

public class Room
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}