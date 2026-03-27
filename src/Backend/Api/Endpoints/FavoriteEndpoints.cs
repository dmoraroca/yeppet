using Microsoft.AspNetCore.Http.HttpResults;
using YepPet.Application.Favorites;

namespace YepPet.Api.Endpoints;

internal static class FavoriteEndpoints
{
    public static IEndpointRouteBuilder MapFavoriteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/favorites");

        group.MapGet("/{ownerUserId:guid}", GetByOwnerAsync);
        group.MapPost("/{ownerUserId:guid}/places/{placeId:guid}", SavePlaceAsync);
        group.MapDelete("/{ownerUserId:guid}/places/{placeId:guid}", RemovePlaceAsync);

        return app;
    }

    private static async Task<Results<Ok<FavoriteListDto>, NotFound>> GetByOwnerAsync(
        Guid ownerUserId,
        IFavoriteListApplicationService service,
        CancellationToken cancellationToken)
    {
        var favoriteList = await service.GetByOwnerAsync(ownerUserId, cancellationToken);
        return favoriteList is null ? TypedResults.NotFound() : TypedResults.Ok(favoriteList);
    }

    private static async Task<Ok<Guid>> SavePlaceAsync(
        Guid ownerUserId,
        Guid placeId,
        IFavoriteListApplicationService service,
        CancellationToken cancellationToken)
    {
        var favoriteListId = await service.SavePlaceAsync(ownerUserId, placeId, cancellationToken);
        return TypedResults.Ok(favoriteListId);
    }

    private static async Task<NoContent> RemovePlaceAsync(
        Guid ownerUserId,
        Guid placeId,
        IFavoriteListApplicationService service,
        CancellationToken cancellationToken)
    {
        await service.RemovePlaceAsync(ownerUserId, placeId, cancellationToken);
        return TypedResults.NoContent();
    }
}
