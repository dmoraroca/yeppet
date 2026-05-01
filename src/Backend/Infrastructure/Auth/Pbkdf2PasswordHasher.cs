using System.Security.Cryptography;
using System.Text;
using Zuppeto.Application.Auth;

namespace Zuppeto.Infrastructure.Auth;

internal sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);

        return $"pbkdf2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(providedPassword))
        {
            return false;
        }

        var parts = hashedPassword.Split('$', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 4 && string.Equals(parts[0], "pbkdf2", StringComparison.OrdinalIgnoreCase))
        {
            var iterations = int.Parse(parts[1]);
            var salt = Convert.FromBase64String(parts[2]);
            var expectedKey = Convert.FromBase64String(parts[3]);
            var actualKey = Rfc2898DeriveBytes.Pbkdf2(
                providedPassword,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(hashedPassword.Trim()),
            Encoding.UTF8.GetBytes(providedPassword.Trim()));
    }
}
