using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Zuppeto.Application.Auth;

namespace Zuppeto.Infrastructure.Auth;

internal sealed class LinkedInOAuthClient(
    HttpClient httpClient,
    IOptions<AuthOptions> options,
    ILogger<LinkedInOAuthClient> logger) : ILinkedInOAuthClient
{
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(options.Value.LinkedIn.ClientId) &&
        !string.IsNullOrWhiteSpace(options.Value.LinkedIn.ClientSecret) &&
        !string.IsNullOrWhiteSpace(options.Value.LinkedIn.RedirectUri) &&
        !string.IsNullOrWhiteSpace(options.Value.FrontendBaseUrl);

    public string? ClientId => string.IsNullOrWhiteSpace(options.Value.LinkedIn.ClientId)
        ? null
        : options.Value.LinkedIn.ClientId.Trim();

    public IReadOnlyCollection<string> AdminEmails => options.Value.LinkedIn.AdminEmails
        .Where(email => !string.IsNullOrWhiteSpace(email))
        .Select(email => email.Trim().ToLowerInvariant())
        .ToArray();

    public string? BuildAuthorizationUrl(string? redirectTo = null)
    {
        if (!IsConfigured || ClientId is null)
        {
            return null;
        }

        var state = ProtectState(redirectTo);
        return
            $"https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id={Uri.EscapeDataString(ClientId)}&redirect_uri={Uri.EscapeDataString(options.Value.LinkedIn.RedirectUri)}&state={Uri.EscapeDataString(state)}&scope={Uri.EscapeDataString("openid profile email")}";
    }

    public async Task<(FederatedIdentityPayload Identity, string? RedirectTo)?> ExchangeCodeAsync(
        string code,
        string state,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || ClientId is null)
        {
            logger.LogWarning("LinkedIn OAuth requested but provider is not fully configured.");
            return null;
        }

        var statePayload = UnprotectState(state);
        if (statePayload is null)
        {
            logger.LogWarning("LinkedIn OAuth state could not be validated.");
            return null;
        }
        var redirectTo = statePayload.RedirectTo;

        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://www.linkedin.com/oauth/v2/accessToken")
        {
            Content = new FormUrlEncodedContent(
            [
                KeyValuePair.Create("grant_type", "authorization_code"),
                KeyValuePair.Create("code", code),
                KeyValuePair.Create("redirect_uri", options.Value.LinkedIn.RedirectUri),
                KeyValuePair.Create("client_id", ClientId),
                KeyValuePair.Create("client_secret", options.Value.LinkedIn.ClientSecret)
            ])
        };

        using var tokenResponse = await httpClient.SendAsync(tokenRequest, cancellationToken);
        if (!tokenResponse.IsSuccessStatusCode)
        {
            logger.LogWarning("LinkedIn token exchange failed with status {StatusCode}.", tokenResponse.StatusCode);
            return null;
        }

        var tokenPayload = await tokenResponse.Content.ReadFromJsonAsync<LinkedInTokenResponse>(cancellationToken);
        if (tokenPayload is null || string.IsNullOrWhiteSpace(tokenPayload.AccessToken))
        {
            logger.LogWarning("LinkedIn token exchange returned no access token.");
            return null;
        }

        var idTokenClaims = ParseIdTokenClaims(tokenPayload.IdToken);

        using var profileRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.linkedin.com/v2/userinfo");
        profileRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenPayload.AccessToken);

        using var profileResponse = await httpClient.SendAsync(profileRequest, cancellationToken);
        LinkedInUserInfoResponse? profilePayload = null;

        if (profileResponse.IsSuccessStatusCode)
        {
            profilePayload = await profileResponse.Content.ReadFromJsonAsync<LinkedInUserInfoResponse>(cancellationToken);
        }
        else
        {
            logger.LogWarning("LinkedIn userinfo call failed with status {StatusCode}.", profileResponse.StatusCode);
        }

        var subject = profilePayload?.Sub ?? idTokenClaims?.Sub;
        var email = profilePayload?.Email ?? idTokenClaims?.Email;

        if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(email))
        {
            logger.LogWarning(
                "LinkedIn identity incomplete. subjectPresent={SubjectPresent} emailPresent={EmailPresent} profileNamePresent={ProfileNamePresent} idTokenPresent={IdTokenPresent}",
                !string.IsNullOrWhiteSpace(subject),
                !string.IsNullOrWhiteSpace(email),
                !string.IsNullOrWhiteSpace(profilePayload?.Name),
                !string.IsNullOrWhiteSpace(tokenPayload.IdToken));
            return null;
        }

        logger.LogInformation(
            "LinkedIn identity accepted for email {Email}. profileEmailPresent={ProfileEmailPresent} idTokenEmailPresent={IdTokenEmailPresent}",
            email.Trim(),
            !string.IsNullOrWhiteSpace(profilePayload?.Email),
            !string.IsNullOrWhiteSpace(idTokenClaims?.Email));

        return (
            new FederatedIdentityPayload(
                "linkedin",
                subject.Trim(),
                email.Trim(),
                ResolveDisplayName(profilePayload, idTokenClaims, email),
                profilePayload?.Picture ?? idTokenClaims?.Picture,
                profilePayload?.EmailVerified ?? idTokenClaims?.EmailVerified ?? true),
            redirectTo);
    }

    private string ProtectState(string? redirectTo)
    {
        var payload = JsonSerializer.Serialize(new LinkedInStatePayload(
            RandomNumberGenerator.GetHexString(16),
            redirectTo,
            DateTimeOffset.UtcNow));
        var encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(payload));
        var signature = ComputeStateSignature(encodedPayload);
        return $"{encodedPayload}.{signature}";
    }

    private LinkedInStatePayload? UnprotectState(string protectedState)
    {
        try
        {
            var separatorIndex = protectedState.LastIndexOf('.');
            if (separatorIndex <= 0 || separatorIndex == protectedState.Length - 1)
            {
                return null;
            }

            var encodedPayload = protectedState[..separatorIndex];
            var signature = protectedState[(separatorIndex + 1)..];
            var expectedSignature = ComputeStateSignature(encodedPayload);

            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(signature),
                    Encoding.UTF8.GetBytes(expectedSignature)))
            {
                return null;
            }

            var json = Encoding.UTF8.GetString(Base64UrlDecode(encodedPayload));
            var payload = JsonSerializer.Deserialize<LinkedInStatePayload>(json);

            if (payload is null || payload.IssuedAtUtc < DateTimeOffset.UtcNow.AddMinutes(-15))
            {
                return null;
            }

            return payload;
        }
        catch
        {
            return null;
        }
    }

    private static string ResolveDisplayName(
        LinkedInUserInfoResponse? profilePayload,
        LinkedInIdTokenClaims? idTokenClaims,
        string email)
    {
        var candidate = profilePayload?.Name ?? idTokenClaims?.Name;
        return string.IsNullOrWhiteSpace(candidate) ? email.Trim() : candidate.Trim();
    }

    private static LinkedInIdTokenClaims? ParseIdTokenClaims(string? idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            return null;
        }

        var segments = idToken.Split('.');
        if (segments.Length < 2)
        {
            return null;
        }

        try
        {
            var json = DecodeBase64Url(segments[1]);
            return JsonSerializer.Deserialize<LinkedInIdTokenClaims>(json);
        }
        catch
        {
            return null;
        }
    }

    private static string DecodeBase64Url(string value)
    {
        return Encoding.UTF8.GetString(Base64UrlDecode(value));
    }

    private string ComputeStateSignature(string encodedPayload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(options.Value.Jwt.SigningKey));
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(encodedPayload)));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var normalized = value.Replace('-', '+').Replace('_', '/');
        var padded = normalized.PadRight(normalized.Length + (4 - normalized.Length % 4) % 4, '=');
        return Convert.FromBase64String(padded);
    }

    private sealed record LinkedInStatePayload(string Nonce, string? RedirectTo, DateTimeOffset IssuedAtUtc);

    private sealed record LinkedInTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn,
        [property: JsonPropertyName("id_token")] string? IdToken);

    private sealed record LinkedInUserInfoResponse(
        [property: JsonPropertyName("sub")] string Sub,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("email_verified")] bool? EmailVerified,
        [property: JsonPropertyName("picture")] string? Picture);

    private sealed record LinkedInIdTokenClaims(
        [property: JsonPropertyName("sub")] string? Sub,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("email")] string? Email,
        [property: JsonPropertyName("email_verified")] bool? EmailVerified,
        [property: JsonPropertyName("picture")] string? Picture);
}
