namespace YepPet.Application.Auth;

public interface IFacebookOAuthClient
{
    bool IsConfigured { get; }

    string? AppId { get; }

    IReadOnlyCollection<string> AdminEmails { get; }

    string? BuildAuthorizationUrl(string? redirectTo = null);

    Task<(FederatedIdentityPayload Identity, string? RedirectTo)?> ExchangeCodeAsync(
        string code,
        string state,
        CancellationToken cancellationToken = default);
}
