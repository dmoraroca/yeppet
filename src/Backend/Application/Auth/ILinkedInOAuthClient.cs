namespace Zuppeto.Application.Auth;

public interface ILinkedInOAuthClient
{
    bool IsConfigured { get; }

    string? ClientId { get; }

    IReadOnlyCollection<string> AdminEmails { get; }

    string? BuildAuthorizationUrl(string? redirectTo = null);

    Task<(FederatedIdentityPayload Identity, string? RedirectTo)?> ExchangeCodeAsync(
        string code,
        string state,
        CancellationToken cancellationToken = default);
}
