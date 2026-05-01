using Microsoft.EntityFrameworkCore;
using Zuppeto.Domain.Abstractions;
using Zuppeto.Domain.Favorites;
using Zuppeto.Infrastructure.Persistence.Mappings;

namespace Zuppeto.Infrastructure.Persistence.Repositories;

internal sealed class FavoriteListRepository(ZuppetoDbContext dbContext) : IFavoriteListRepository
{
    public async Task<FavoriteList?> GetByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        var record = await dbContext.FavoriteLists
            .AsNoTracking()
            .Include(favoriteList => favoriteList.Entries)
            .FirstOrDefaultAsync(favoriteList => favoriteList.OwnerUserId == ownerUserId, cancellationToken);

        return record is null ? null : FavoriteListPersistenceMapper.ToDomain(record);
    }

    public Task<bool> ExistsByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        return dbContext.FavoriteLists.AnyAsync(favoriteList => favoriteList.OwnerUserId == ownerUserId, cancellationToken);
    }

    public async Task AddAsync(FavoriteList favoriteList, CancellationToken cancellationToken = default)
    {
        var record = FavoriteListPersistenceMapper.ToRecord(favoriteList);

        await dbContext.FavoriteLists.AddAsync(record, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FavoriteList favoriteList, CancellationToken cancellationToken = default)
    {
        var record = await dbContext.FavoriteLists
            .Include(current => current.Entries)
            .FirstOrDefaultAsync(current => current.Id == favoriteList.Id, cancellationToken);

        if (record is null)
        {
            throw new InvalidOperationException($"Favorite list '{favoriteList.Id}' was not found.");
        }

        FavoriteListPersistenceMapper.Apply(favoriteList, record);
        FavoriteListPersistenceMapper.SyncEntries(favoriteList, record);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
