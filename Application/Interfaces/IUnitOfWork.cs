using Domain.Entities;
namespace Application.Interfaces;
public interface IUnitOfWork
{
    IRepository<Company> Companies { get; }
    IRepository<User> Users { get; }
    IRepository<Room> Rooms { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<WorkingHours> WorkingHours { get; }
    IRepository<AuditLog> AuditLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
public interface IRepository<T> where T: class
{
    IQueryable<T> Query();
    Task<T?> GetAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
public record CreateCompanyDto(string Name, string Password, string AdminDisplayName, long? AdminTelegramId);
