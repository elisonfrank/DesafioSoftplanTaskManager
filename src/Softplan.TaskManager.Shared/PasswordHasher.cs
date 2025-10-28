using System.Security.Cryptography;

namespace Softplan.TaskManager.Shared;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Gera um salt aleatório
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // Deriva a chave com PBKDF2
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000, // número de iterações
            HashAlgorithmName.SHA256,
            32);

        // Combina salt + hash em Base64
        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }
    
    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256,
            32);

        return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
    }
}