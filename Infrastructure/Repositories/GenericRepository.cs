using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;
public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _db;
    public GenericRepository(AppDbContext db) { _db = db; }
    public IQueryable<T> Query() => _db.Set<T>().AsQueryable();
    public Task<T?> GetAsync(Guid id) => _db.Set<T>().FindAsync(id).AsTask();
    public Task AddAsync(T entity) { _db.Set<T>().Add(entity); return Task.CompletedTask; }
    public void Update(T entity) { _db.Set<T>().Update(entity); }
    public void Remove(T entity) { _db.Set<T>().Remove(entity); }
}