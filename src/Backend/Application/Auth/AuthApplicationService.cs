using YepPet.Application.Users;
using YepPet.Domain.Abstractions;
using YepPet.Domain.Users;
using YepPet.Domain.Users.ValueObjects;

namespace YepPet.Application.Auth;

internal sealed class AuthApplicationService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IAccessTokenIssuer accessTokenIssuer,
    IGoogleIdTokenVerifier googleIdTokenVerifier) : IAuthApplicationService
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

    public async Task<AuthSessionDto?> LoginWithGoogleAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var identity = await googleIdTokenVerifier.VerifyAsync(request.IdToken, cancellationToken);

        if (identity is null || !identity.EmailVerified)
        {
            return null;
        }

        var user = await userRepository.GetByEmailAsync(identity.Email, cancellationToken);
        var shouldBeAdmin = IsAdminEmail(identity.Email);

        if (user is null)
        {
            user = new User(
                Guid.NewGuid(),
                identity.Email,
                passwordHasher.Hash(Guid.NewGuid().ToString("N")),
                shouldBeAdmin ? UserRole.Admin : UserRole.User,
                new UserProfile(
                    string.IsNullOrWhiteSpace(identity.DisplayName) ? identity.Email : identity.DisplayName,
                    string.Empty,
                    string.Empty,
                    "Perfil creat a través de Google.",
                    identity.AvatarUrl),
                shouldBeAdmin
                    ? new PrivacyConsent(true, DateTimeOffset.UtcNow)
                    : new PrivacyConsent(false, null));

            await userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            var shouldPersist = false;

            if (shouldBeAdmin && user.Role != UserRole.Admin)
            {
                user.ChangeRole(UserRole.Admin);

                if (!user.PrivacyConsent.Accepted)
                {
                    user.AcceptPrivacy(DateTimeOffset.UtcNow);
                }

                shouldPersist = true;
            }

            if (CanSynchronizeProfile(user) && ShouldSynchronizeProfile(user, identity))
            {
                user.UpdateProfile(
                    new UserProfile(
                        ResolveDisplayName(identity),
                        user.Profile.City,
                        user.Profile.Country,
                        string.IsNullOrWhiteSpace(user.Profile.Bio)
                            ? "Perfil sincronitzat a través de Google."
                            : user.Profile.Bio,
                        string.IsNullOrWhiteSpace(identity.AvatarUrl) ? user.Profile.AvatarUrl : identity.AvatarUrl));
                shouldPersist = true;
            }

            if (shouldPersist)
            {
                await userRepository.UpdateAsync(user, cancellationToken);
            }
        }

        return CreateSession(user, "google");
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
            new("google", "Google", "oidc", googleIdTokenVerifier.IsConfigured, googleIdTokenVerifier.ClientId),
            new("linkedin", "LinkedIn", "oauth2", false),
            new("facebook", "Facebook", "oauth2", false)
        ];
    }

    private AuthSessionDto CreateSession(User user, string provider = "password")
    {
        var token = accessTokenIssuer.Issue(user);

        return new AuthSessionDto(
            token.Token,
            token.ExpiresAtUtc,
            provider,
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

    private bool IsAdminEmail(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return googleIdTokenVerifier.AdminEmails.Any(candidate => candidate == normalizedEmail);
    }

    private static string ResolveDisplayName(FederatedIdentityPayload identity)
    {
        return string.IsNullOrWhiteSpace(identity.DisplayName) ? identity.Email : identity.DisplayName;
    }

    private static bool CanSynchronizeProfile(User user)
    {
        return user.Role == UserRole.Admin || user.PrivacyConsent.Accepted;
    }

    private static bool ShouldSynchronizeProfile(User user, FederatedIdentityPayload identity)
    {
        var nextDisplayName = ResolveDisplayName(identity);
        var nextAvatarUrl = string.IsNullOrWhiteSpace(identity.AvatarUrl) ? user.Profile.AvatarUrl : identity.AvatarUrl;

        return !string.Equals(user.Profile.DisplayName, nextDisplayName, StringComparison.Ordinal) ||
               !string.Equals(user.Profile.AvatarUrl, nextAvatarUrl, StringComparison.Ordinal);
    }
}
