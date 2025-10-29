namespace Domain.Entities;
public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string HashedPassword { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}