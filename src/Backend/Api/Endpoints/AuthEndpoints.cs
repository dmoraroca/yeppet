using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;
using YepPet.Application.Auth;

namespace YepPet.Api.Endpoints;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", LoginAsync);
        group.MapPost("/google", GoogleLoginAsync);
        group.MapGet("/facebook/start", FacebookStartAsync);
        group.MapGet("/facebook/callback", FacebookCallbackAsync);
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

    private static async Task<Results<Ok<AuthSessionDto>, UnauthorizedHttpResult>> GoogleLoginAsync(
        GoogleLoginRequest request,
        IAuthApplicationService service,
        CancellationToken cancellationToken)
    {
        var session = await service.LoginWithGoogleAsync(request, cancellationToken);
        return session is null ? TypedResults.Unauthorized() : TypedResults.Ok(session);
    }

    private static Ok<IReadOnlyCollection<AuthProviderDto>> GetProviders(IAuthApplicationService service)
    {
        return TypedResults.Ok(service.GetProviders());
    }

    private static Results<RedirectHttpResult, NotFound> FacebookStartAsync(
        IAuthApplicationService service,
        string? redirectTo = null)
    {
        var authorizationUrl = service.GetFacebookAuthorizationUrl(redirectTo);
        return string.IsNullOrWhiteSpace(authorizationUrl)
            ? TypedResults.NotFound()
            : TypedResults.Redirect(authorizationUrl);
    }

    private static async Task<RedirectHttpResult> FacebookCallbackAsync(
        IAuthApplicationService service,
        IConfiguration configuration,
        string? code = null,
        string? state = null,
        string? error = null,
        string? error_reason = null,
        CancellationToken cancellationToken = default)
    {
        var frontendBaseUrl = configuration["Auth:FrontendBaseUrl"] ?? "http://localhost:4200";
        var loginUrl = $"{frontendBaseUrl.TrimEnd('/')}/login";

        if (!string.IsNullOrWhiteSpace(error) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
        {
            return TypedResults.Redirect(
                QueryHelpers.AddQueryString(
                    loginUrl,
                    "federatedError",
                    string.IsNullOrWhiteSpace(error_reason) ? "facebook-login-failed" : error_reason));
        }

        var result = await service.LoginWithFacebookAsync(new FacebookOAuthCallbackRequest(code, state), cancellationToken);
        if (result is null)
        {
            return TypedResults.Redirect(QueryHelpers.AddQueryString(loginUrl, "federatedError", "facebook-login-failed"));
        }

        var serializedSession = JsonSerializer.Serialize(result.Session);
        var sessionPayload = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(serializedSession));
        var callbackUrl = QueryHelpers.AddQueryString(
            $"{frontendBaseUrl.TrimEnd('/')}/auth/callback",
            new Dictionary<string, string?>()
            {
                ["session"] = sessionPayload,
                ["redirectTo"] = result.RedirectTo
            });

        return TypedResults.Redirect(callbackUrl);
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
