using YepPet.Application.Users;
using YepPet.Domain.Abstractions;
using YepPet.Domain.Users;

namespace YepPet.Application.Auth;

internal sealed class AuthApplicationService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IAccessTokenIssuer accessTokenIssuer) : IAuthApplicationService
{
    public async Task<AuthSessionDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return null;
        }

        return CreateSession(user);
    }

    public async Task<AuthSessionDto?> GetSessionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        return user is null ? null : CreateSession(user);
    }

    public IReadOnlyCollection<AuthProviderDto> GetProviders()
    {
        return
        [
            new("password", "Credencials pròpies", "password", true),
            new("google", "Google", "oidc", false),
            new("linkedin", "LinkedIn", "oauth2", false),
            new("facebook", "Facebook", "oauth2", false)
        ];
    }

    private AuthSessionDto CreateSession(User user)
    {
        var token = accessTokenIssuer.Issue(user);

        return new AuthSessionDto(
            token.Token,
            token.ExpiresAtUtc,
            "password",
            new UserDto(
                user.Id,
                user.Email,
                user.Role.ToString(),
                user.Profile.DisplayName,
                user.Profile.City,
                user.Profile.Country,
                user.Profile.Bio,
                user.Profile.AvatarUrl,
                user.PrivacyConsent.Accepted,
                user.PrivacyConsent.AcceptedAtUtc));
    }
}
