using System.Security.Cryptography;
using System.Text;

namespace NTH.Utilities;

public static class PasswordHasher
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="password">password clear text, no check here</param>
    /// <param name="salt">pass null to generate a salt or pass existing salt</param>
    /// <returns>hashed byte array of password</returns>
    public static byte[] GetHashedPassword(string password, ref string? salt)
    {
        if (string.IsNullOrEmpty(salt))
        {
            salt = Random5ASCII.GetString();
        }
        byte[] hash = Encoding.UTF8.GetBytes(salt + password);
        for (int i = 0; i < 5; i++)
        {
            hash = SHA256.HashData(hash);
        }
        return hash;
    }
}
