using Account.Core.Entities;
using Account.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence.Repositories;

public class AuthSessionRepository(UsersDbContext context) : IAuthSessionRepository
{
    private readonly UsersDbContext _context = context;

    public async Task AddAsync(AuthSession entity, CancellationToken token = default)
    {
        await _context.AuthSessions.AddAsync(entity, token);
    }

    public void Delete(AuthSession entity)
    {
        _context.AuthSessions.Remove(entity);
    }

    public async Task<IEnumerable<AuthSession>> GetAllAsync(CancellationToken token = default)
    {
        return await _context.AuthSessions.ToListAsync(token);
    }

    public async Task<AuthSession?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _context.AuthSessions.FindAsync([id], token);
    }

    public void Update(AuthSession entity)
    {
        _context.AuthSessions.Update(entity);
    }
}
