using Account.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Account.Application.Common.Interfaces;

public interface IJwtTokenService
{
    //NOTE: Add roles in future
    string GenerateAccessToken(Guid userId, string email);
    RefreshToken GenerateRefreshToken(Guid userId);
    Task<TokenValidationResult> ValidateTokenAsync(string token);
}
