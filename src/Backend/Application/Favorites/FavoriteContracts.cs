namespace YepPet.Application.Favorites;

public sealed record FavoriteEntryDto(Guid Id, Guid PlaceId, DateTimeOffset SavedAtUtc);

public sealed record FavoriteListDto(Guid Id, Guid OwnerUserId, IReadOnlyCollection<FavoriteEntryDto> Entries);
