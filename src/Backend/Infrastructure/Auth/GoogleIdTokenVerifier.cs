using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using YepPet.Application.Auth;

namespace YepPet.Infrastructure.Auth;

internal sealed class GoogleIdTokenVerifier(IOptions<AuthOptions> options) : IGoogleIdTokenVerifier
{
    public bool IsConfigured => !string.IsNullOrWhiteSpace(ClientId);

    public string? ClientId => string.IsNullOrWhiteSpace(options.Value.Google.ClientId)
        ? null
        : options.Value.Google.ClientId.Trim();

    public IReadOnlyCollection<string> AdminEmails => options.Value.Google.AdminEmails
        .Where(email => !string.IsNullOrWhiteSpace(email))
        .Select(email => email.Trim().ToLowerInvariant())
        .Distinct()
        .ToArray();

    public async Task<FederatedIdentityPayload?> VerifyAsync(string idToken, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || string.IsNullOrWhiteSpace(idToken))
        {
            return null;
        }

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [ClientId!]
                });

            return new FederatedIdentityPayload(
                "google",
                payload.Subject,
                payload.Email,
                string.IsNullOrWhiteSpace(payload.Name) ? payload.Email : payload.Name,
                payload.Picture,
                payload.EmailVerified);
        }
        catch
        {
            return null;
        }
    }
}
