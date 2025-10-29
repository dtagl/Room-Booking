using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts): base(opts) {}

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<WorkingHours> WorkingHours => Set<WorkingHours>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        mb.Entity<Company>().HasIndex(c => c.Name);
        mb.Entity<User>().HasIndex(u => new { u.CompanyId, u.TelegramId });
        mb.Entity<Room>().HasIndex(r => new { r.CompanyId, r.Name });
        mb.Entity<Booking>().HasIndex(b => new { b.RoomId, b.StartAt, b.EndAt });
        // Booking overlap protection to be enforced in app logic / DB triggers if needed
    }
}