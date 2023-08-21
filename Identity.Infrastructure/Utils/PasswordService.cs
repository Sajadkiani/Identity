using System.Text;
using Identity.Domain.IServices;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Identity.Infrastructure.Utils;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password, string userName)
    {
        var salt = GetPasswordSalt(password, userName);
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
    }
    
    private static byte[] GetPasswordSalt(string password, string userName)
    {
        byte[] salt = Encoding.ASCII.GetBytes($"{userName}{password}");
        return salt;
    }
}