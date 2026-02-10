using Account.Core.Entities;
using Account.Core.Repositories.Common;

namespace Account.Core.Repositories;

public interface IEmailVerificationCodeRepository : IRepository<EmailVerificationCode>
{
    Task<EmailVerificationCode?> GetActiveByEmailAsync(string email, CancellationToken token);
    Task UseCodeByEmailAsync(string email, CancellationToken token);
}
