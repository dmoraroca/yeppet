namespace YepPet.Application.Auth;

public interface IAuthApplicationService
{
    Task<AuthSessionDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthSessionDto?> GetSessionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    IReadOnlyCollection<AuthProviderDto> GetProviders();
}
