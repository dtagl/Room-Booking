namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public long? TelegramId { get; set; }
    public string? Email { get; set; }
    public string DisplayName { get; set; } = default!;
    public Role Role { get; set; } = Role.User;
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}