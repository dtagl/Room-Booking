using Application.Interfaces;
using Domain.Entities;
namespace Application.Services;
public class AuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    public AuthService(IUnitOfWork uow, IPasswordHasher hasher) { _uow = uow; _hasher = hasher; }
    public async Task<(Company company, User admin)> CreateCompanyAsync(string name, string password, string adminDisplayName, long? adminTelegramId)
    {
        var company = new Company{ Id = Guid.NewGuid(), Name = name, HashedPassword = _hasher.Hash(password), CreatedAt = DateTime.UtcNow };
        await _uow.Companies.AddAsync(company);
        var admin = new User{ Id = Guid.NewGuid(), DisplayName = adminDisplayName, CompanyId = company.Id, Role = Role.Admin, TelegramId = adminTelegramId, CreatedAt = DateTime.UtcNow };
        await _uow.Users.AddAsync(admin);
        await _uow.SaveChangesAsync();
        return (company, admin);
    }
    public async Task<User?> LoginToCompanyAsync(string name, string password, long? telegramId, string displayName)
    {
        var company = _uow.Companies.Query().FirstOrDefault(c => c.Name == name);
        if (company == null || !_hasher.Verify(company.HashedPassword, password))
            return null;

        var user = _uow.Users.Query().FirstOrDefault(u => u.CompanyId == company.Id && u.TelegramId == telegramId);
        if (user == null)
        {
            // Создаём нового пользователя
            user = new User
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                TelegramId = telegramId,
                DisplayName = displayName,
                Role = Role.User,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();
        }

        return user;
    }

}