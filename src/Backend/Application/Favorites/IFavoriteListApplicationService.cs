namespace Zuppeto.Application.Favorites;

public interface IFavoriteListApplicationService
{
    Task<FavoriteListDto?> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default);

    Task<Guid> SavePlaceAsync(Guid ownerUserId, Guid placeId, CancellationToken cancellationToken = default);

    Task RemovePlaceAsync(Guid ownerUserId, Guid placeId, CancellationToken cancellationToken = default);
}
