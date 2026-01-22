using Account.Core.Entities;
using Account.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence.Repositories;

public class PlayerRepository(UserDbContext context) : IPlayerRepository
{
    private readonly UserDbContext _context = context;

    public async Task AddAsync(Player entity, CancellationToken token = default)
    {
        await _context.Players.AddAsync(entity, token);
    }

    public void Delete(Player entity)
    {
        _context.Players.Remove(entity);
    }

    public async Task<IEnumerable<Player>> GetAllAsync(CancellationToken token = default)
    {
       return await _context.Players.ToListAsync(token);
    }

    public Task<Player?> GetByEmailAsync(string email, CancellationToken token = default)
    {
        return _context.Players.FirstOrDefaultAsync(p => p.Email == email, token);
    }

    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _context.Players.FindAsync([id], token);
    }

    public async Task<bool> IsExistsByEmail(string email, CancellationToken token = default)
    {
        return await _context.Players.AnyAsync(p => p.Email == email, token);
    }

    public void Update(Player entity)
    {
        _context.Update(entity);
    }
}
