namespace Account.Core.Security;

public interface IVerificationCodeHasher
{
    string Hash(string verificationCode);
    bool Verify(string verificationCode, string hash);
}
