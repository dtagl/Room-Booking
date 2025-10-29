using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Companies = new Repositories.GenericRepository<Company>(_db);
        Users = new Repositories.GenericRepository<User>(_db);
        Rooms = new Repositories.GenericRepository<Room>(_db);
        Bookings = new Repositories.GenericRepository<Booking>(_db);
        WorkingHours = new Repositories.GenericRepository<WorkingHours>(_db);
        AuditLogs = new Repositories.GenericRepository<AuditLog>(_db);
    }

    public IRepository<Company> Companies { get; }
    public IRepository<User> Users { get; }
    public IRepository<Room> Rooms { get; }
    public IRepository<Booking> Bookings { get; }
    public IRepository<WorkingHours> WorkingHours { get; }
    public IRepository<AuditLog> AuditLogs { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}