using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using YepPet.Application.Auth;

namespace YepPet.Api.Endpoints;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", LoginAsync);
        group.MapGet("/providers", GetProviders);
        group.MapGet("/me", GetCurrentSessionAsync).RequireAuthorization();

        return app;
    }

    private static async Task<Results<Ok<AuthSessionDto>, UnauthorizedHttpResult>> LoginAsync(
        LoginRequest request,
        IAuthApplicationService service,
        CancellationToken cancellationToken)
    {
        var session = await service.LoginAsync(request, cancellationToken);
        return session is null ? TypedResults.Unauthorized() : TypedResults.Ok(session);
    }

    private static Ok<IReadOnlyCollection<AuthProviderDto>> GetProviders(IAuthApplicationService service)
    {
        return TypedResults.Ok(service.GetProviders());
    }

    [Authorize]
    private static async Task<Results<Ok<AuthSessionDto>, UnauthorizedHttpResult, NotFound>> GetCurrentSessionAsync(
        ClaimsPrincipal principal,
        IAuthApplicationService service,
        CancellationToken cancellationToken)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var session = await service.GetSessionByUserIdAsync(userId, cancellationToken);
        return session is null ? TypedResults.NotFound() : TypedResults.Ok(session);
    }
}
