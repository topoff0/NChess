using System.Security.Cryptography;
using System.Text;
using Account.Core.Security;

namespace Account.Infrastructure.Security;

public class VerificationCodeHasher : IVerificationCodeHasher
{
    public string Hash(string verificationCode)
    {
        //HACK: Make better algorithm
        var bytes = Encoding.UTF8.GetBytes(verificationCode);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    public bool Verify(string verificationCode, string hash)
        => Hash(verificationCode) == hash;
}
