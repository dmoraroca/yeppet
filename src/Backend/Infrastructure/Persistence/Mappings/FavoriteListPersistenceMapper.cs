using YepPet.Domain.Favorites;
using YepPet.Infrastructure.Persistence.Entities;

namespace YepPet.Infrastructure.Persistence.Mappings;

internal static class FavoriteListPersistenceMapper
{
    public static FavoriteList ToDomain(FavoriteListRecord record)
    {
        var favoriteList = new FavoriteList(record.Id, record.OwnerUserId);

        foreach (var entry in record.Entries.OrderBy(entry => entry.SavedAtUtc))
        {
            favoriteList.AddPlace(entry.PlaceId, entry.SavedAtUtc);
        }

        return favoriteList;
    }

    public static FavoriteListRecord ToRecord(FavoriteList favoriteList)
    {
        var record = new FavoriteListRecord();
        Apply(favoriteList, record);
        SyncEntries(favoriteList, record);
        return record;
    }

    public static void Apply(FavoriteList favoriteList, FavoriteListRecord record)
    {
        record.Id = favoriteList.Id;
        record.OwnerUserId = favoriteList.OwnerUserId;
    }

    public static void SyncEntries(FavoriteList favoriteList, FavoriteListRecord record)
    {
        var desiredEntries = favoriteList.Entries.ToDictionary(entry => entry.Id);

        var toRemove = record.Entries
            .Where(entry => !desiredEntries.ContainsKey(entry.Id))
            .ToArray();

        foreach (var entry in toRemove)
        {
            record.Entries.Remove(entry);
        }

        foreach (var entry in favoriteList.Entries)
        {
            var existing = record.Entries.FirstOrDefault(current => current.Id == entry.Id);
            if (existing is null)
            {
                record.Entries.Add(new FavoriteEntryRecord
                {
                    Id = entry.Id,
                    FavoriteListId = favoriteList.Id,
                    PlaceId = entry.PlaceId,
                    SavedAtUtc = entry.SavedAtUtc
                });
                continue;
            }

            existing.PlaceId = entry.PlaceId;
            existing.SavedAtUtc = entry.SavedAtUtc;
        }
    }
}
