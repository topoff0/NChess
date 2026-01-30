using Account.Core.Entities;
using Account.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence.Repositories;

public class EmailVerificationRepository(UsersDbContext context) : IEmailVerificationCodeRepository
{
    private readonly UsersDbContext _context = context;

    public async Task AddAsync(EmailVerificationCode entity, CancellationToken token = default)
    {
        var activeCodes = await _context.EmailVerificationCodes
            .Where(c =>
                    c.Email == entity.Email
                    && c.ExpiryAt > DateTime.UtcNow
                    && c.IsManuallyDeactivated == false)
            .ToListAsync(token);

        foreach (var code in activeCodes)
        {
            code.Deactivate();
        }


        await _context.EmailVerificationCodes.AddAsync(entity, token);
    }

    public void Delete(EmailVerificationCode entity)
    {
        _context.EmailVerificationCodes.Remove(entity);
    }

    public async Task<EmailVerificationCode?> GetActiveByEmailAsync(string email, CancellationToken token)
    {
        return await _context.EmailVerificationCodes.FirstOrDefaultAsync(
                c => c.Email == email && c.ExpiryAt > DateTime.UtcNow, token);
    }

    public async Task<IEnumerable<EmailVerificationCode>> GetAllAsync(CancellationToken token = default)
    {
        return await _context.EmailVerificationCodes.ToListAsync(token);
    }

    public async Task<EmailVerificationCode?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _context.EmailVerificationCodes.FindAsync([id], token);
    }

    public void Update(EmailVerificationCode entity)
    {
        _context.EmailVerificationCodes.Update(entity);
    }
}
