using Account.Core.Entities;
using Account.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence.Repositories;

public class UserRepository(UsersDbContext context) : IUserRepository
{
    private readonly UsersDbContext _context = context;

    public async Task AddAsync(User entity, CancellationToken token = default)
    {
        await _context.Users.AddAsync(entity, token);
    }

    public void Delete(User entity)
    {
        _context.Users.Remove(entity);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken token = default)
    {
        return await _context.Users.ToListAsync(token);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken token = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, token);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _context.Users.FindAsync([id], token);
    }

    public async Task<bool> IsExistsAndActiveByEmail(string email, CancellationToken token = default)
    {
        return await _context.Users.AnyAsync(u => u.Email == email && u.Status == UserStatus.Active, token);
    }

    public void Update(User entity)
    {
        _context.Users.Update(entity);
    }
}
