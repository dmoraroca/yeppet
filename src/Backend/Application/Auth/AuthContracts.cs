using YepPet.Application.Users;

namespace YepPet.Application.Auth;

public sealed record LoginRequest(string Email, string Password);

public sealed record GoogleLoginRequest(string IdToken);

public sealed record AuthSessionDto(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    string Provider,
    UserDto User);

public sealed record AuthProviderDto(
    string Key,
    string DisplayName,
    string Protocol,
    bool Configured,
    string? ClientId = null);

public sealed record AccessTokenResult(
    string Token,
    DateTimeOffset ExpiresAtUtc);

public sealed record FederatedIdentityPayload(
    string Provider,
    string Subject,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    bool EmailVerified);
