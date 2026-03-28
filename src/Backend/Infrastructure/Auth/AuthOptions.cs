namespace YepPet.Infrastructure.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public JwtOptions Jwt { get; init; } = new();
    public GoogleOptions Google { get; init; } = new();

    public sealed class JwtOptions
    {
        public string Issuer { get; init; } = "YepPet";

        public string Audience { get; init; } = "YepPet.Web";

        public string SigningKey { get; init; } = "dev-only-yep-pet-signing-key-change-in-production-123456789";

        public int ExpiresInMinutes { get; init; } = 480;
    }

    public sealed class GoogleOptions
    {
        public string ClientId { get; init; } = string.Empty;

        public string[] AdminEmails { get; init; } = [];
    }
}
