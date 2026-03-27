using YepPet.Domain.Abstractions;
using YepPet.Domain.Favorites;

namespace YepPet.Application.Favorites;

internal sealed class FavoriteListApplicationService(IFavoriteListRepository favoriteListRepository) : IFavoriteListApplicationService
{
    public async Task<FavoriteListDto?> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        var favoriteList = await favoriteListRepository.GetByOwnerAsync(ownerUserId, cancellationToken);
        return favoriteList is null ? null : ToDto(favoriteList);
    }

    public async Task<Guid> SavePlaceAsync(Guid ownerUserId, Guid placeId, CancellationToken cancellationToken = default)
    {
        var favoriteList = await favoriteListRepository.GetByOwnerAsync(ownerUserId, cancellationToken);
        if (favoriteList is null)
        {
            favoriteList = new FavoriteList(Guid.NewGuid(), ownerUserId);
            favoriteList.AddPlace(placeId, DateTimeOffset.UtcNow);
            await favoriteListRepository.AddAsync(favoriteList, cancellationToken);
            return favoriteList.Id;
        }

        favoriteList.AddPlace(placeId, DateTimeOffset.UtcNow);
        await favoriteListRepository.UpdateAsync(favoriteList, cancellationToken);
        return favoriteList.Id;
    }

    public async Task RemovePlaceAsync(Guid ownerUserId, Guid placeId, CancellationToken cancellationToken = default)
    {
        var favoriteList = await favoriteListRepository.GetByOwnerAsync(ownerUserId, cancellationToken);
        if (favoriteList is null)
        {
            return;
        }

        favoriteList.RemovePlace(placeId);
        await favoriteListRepository.UpdateAsync(favoriteList, cancellationToken);
    }

    private static FavoriteListDto ToDto(FavoriteList favoriteList)
    {
        return new FavoriteListDto(
            favoriteList.Id,
            favoriteList.OwnerUserId,
            favoriteList.Entries
                .OrderByDescending(entry => entry.SavedAtUtc)
                .Select(entry => new FavoriteEntryDto(entry.Id, entry.PlaceId, entry.SavedAtUtc))
                .ToArray());
    }
}
