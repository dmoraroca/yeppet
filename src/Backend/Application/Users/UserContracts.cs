namespace Zuppeto.Application.Users;

public sealed record UserDto(
    Guid Id,
    string Email,
    string Role,
    string DisplayName,
    string City,
    string Country,
    string Bio,
    string? AvatarUrl,
    bool PrivacyAccepted,
    DateTimeOffset? PrivacyAcceptedAtUtc);

public sealed record UserRegistrationRequest(
    string Email,
    string PasswordHash,
    string Role,
    string DisplayName,
    string City,
    string Country,
    string Bio,
    string? AvatarUrl,
    bool PrivacyAccepted,
    DateTimeOffset? PrivacyAcceptedAtUtc);

public sealed record UserProfileUpdateRequest(
    Guid Id,
    string DisplayName,
    string City,
    string Country,
    string Bio,
    string? AvatarUrl,
    bool PrivacyAccepted,
    DateTimeOffset? PrivacyAcceptedAtUtc);
