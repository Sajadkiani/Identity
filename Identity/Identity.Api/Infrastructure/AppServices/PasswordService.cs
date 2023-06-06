using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Identity.Api.Infrastructure.AppServices;

public interface IPasswordService
{
    string HashPassword(int userId, string password, string userName);
}

public class PasswordService
{
    public string HashPassword(int userId, string password, string userName)
    {
        var salt = GetPasswordSalt(userId, password, userName);
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password!,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
    }
    
    private static byte[] GetPasswordSalt(int userId, string password, string userName)
    {
        byte[] salt = Convert.FromBase64String($"{userId}{userName}{password}");
        return salt;
    }
}