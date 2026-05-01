namespace Zuppeto.Application.Auth;

public interface IGoogleIdTokenVerifier
{
    bool IsConfigured { get; }

    string? ClientId { get; }

    IReadOnlyCollection<string> AdminEmails { get; }

    Task<FederatedIdentityPayload?> VerifyAsync(string idToken, CancellationToken cancellationToken = default);
}
