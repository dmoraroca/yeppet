using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Zuppeto.Application.Auth;

namespace Zuppeto.Infrastructure.Auth;

internal sealed class FacebookOAuthClient(
    HttpClient httpClient,
    IOptions<AuthOptions> options,
    IDataProtectionProvider dataProtectionProvider) : IFacebookOAuthClient
{
    private readonly IDataProtector dataProtector = dataProtectionProvider.CreateProtector("Zuppeto.Auth.Facebook.State");

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(options.Value.Facebook.AppId) &&
        !string.IsNullOrWhiteSpace(options.Value.Facebook.AppSecret) &&
        !string.IsNullOrWhiteSpace(options.Value.Facebook.RedirectUri) &&
        !string.IsNullOrWhiteSpace(options.Value.FrontendBaseUrl);

    public string? AppId => string.IsNullOrWhiteSpace(options.Value.Facebook.AppId)
        ? null
        : options.Value.Facebook.AppId.Trim();

    public IReadOnlyCollection<string> AdminEmails => options.Value.Facebook.AdminEmails
        .Where(email => !string.IsNullOrWhiteSpace(email))
        .Select(email => email.Trim().ToLowerInvariant())
        .ToArray();

    public string? BuildAuthorizationUrl(string? redirectTo = null)
    {
        if (!IsConfigured || AppId is null)
        {
            return null;
        }

        var state = ProtectState(redirectTo);
        return
            $"https://www.facebook.com/v23.0/dialog/oauth?client_id={Uri.EscapeDataString(AppId)}&redirect_uri={Uri.EscapeDataString(options.Value.Facebook.RedirectUri)}&state={Uri.EscapeDataString(state)}&scope={Uri.EscapeDataString("email,public_profile")}&response_type=code";
    }

    public async Task<(FederatedIdentityPayload Identity, string? RedirectTo)?> ExchangeCodeAsync(
        string code,
        string state,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || AppId is null)
        {
            return null;
        }

        var statePayload = UnprotectState(state);
        if (statePayload is null)
        {
            return null;
        }
        var redirectTo = statePayload.RedirectTo;

        var tokenResponse = await httpClient.GetFromJsonAsync<FacebookTokenResponse>(
            $"https://graph.facebook.com/v23.0/oauth/access_token?client_id={Uri.EscapeDataString(AppId)}&redirect_uri={Uri.EscapeDataString(options.Value.Facebook.RedirectUri)}&client_secret={Uri.EscapeDataString(options.Value.Facebook.AppSecret)}&code={Uri.EscapeDataString(code)}",
            cancellationToken);

        if (tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            return null;
        }

        var profileResponse = await httpClient.GetFromJsonAsync<FacebookProfileResponse>(
            $"https://graph.facebook.com/me?fields=id,name,email,picture.type(large)&access_token={Uri.EscapeDataString(tokenResponse.AccessToken)}",
            cancellationToken);

        if (profileResponse is null || string.IsNullOrWhiteSpace(profileResponse.Id) || string.IsNullOrWhiteSpace(profileResponse.Email))
        {
            return null;
        }

        return (
            new FederatedIdentityPayload(
                "facebook",
                profileResponse.Id.Trim(),
                profileResponse.Email.Trim(),
                string.IsNullOrWhiteSpace(profileResponse.Name) ? profileResponse.Email.Trim() : profileResponse.Name.Trim(),
                profileResponse.Picture?.Data?.Url,
                true),
            redirectTo);
    }

    private string ProtectState(string? redirectTo)
    {
        var payload = JsonSerializer.Serialize(new FacebookStatePayload(
            RandomNumberGenerator.GetHexString(16),
            redirectTo,
            DateTimeOffset.UtcNow));

        return dataProtector.Protect(payload);
    }

    private FacebookStatePayload? UnprotectState(string protectedState)
    {
        try
        {
            var json = dataProtector.Unprotect(protectedState);
            var payload = JsonSerializer.Deserialize<FacebookStatePayload>(json);

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

    private sealed record FacebookStatePayload(string Nonce, string? RedirectTo, DateTimeOffset IssuedAtUtc);

    private sealed record FacebookTokenResponse(string AccessToken);

    private sealed record FacebookProfileResponse(string Id, string? Name, string? Email, FacebookPictureResponse? Picture);

    private sealed record FacebookPictureResponse(FacebookPictureDataResponse? Data);

    private sealed record FacebookPictureDataResponse(string? Url);
}
