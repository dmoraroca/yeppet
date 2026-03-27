using Microsoft.AspNetCore.Http.HttpResults;
using YepPet.Application.Users;

namespace YepPet.Api.Endpoints;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/{id:guid}", GetByIdAsync);
        group.MapGet("/by-email/{email}", GetByEmailAsync);
        group.MapPost("/", RegisterAsync);
        group.MapPut("/{id:guid}/profile", UpdateProfileAsync);

        return app;
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> GetByIdAsync(
        Guid id,
        IUserApplicationService service,
        CancellationToken cancellationToken)
    {
        var user = await service.GetByIdAsync(id, cancellationToken);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> GetByEmailAsync(
        string email,
        IUserApplicationService service,
        CancellationToken cancellationToken)
    {
        var user = await service.GetByEmailAsync(email, cancellationToken);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Created<Guid>> RegisterAsync(
        UserRegistrationRequest request,
        IUserApplicationService service,
        CancellationToken cancellationToken)
    {
        var userId = await service.RegisterAsync(request, cancellationToken);
        return TypedResults.Created($"/api/users/{userId}", userId);
    }

    private static async Task<NoContent> UpdateProfileAsync(
        Guid id,
        UserProfileUpdateRequest request,
        IUserApplicationService service,
        CancellationToken cancellationToken)
    {
        await service.UpdateProfileAsync(request with { Id = id }, cancellationToken);
        return TypedResults.NoContent();
    }
}
