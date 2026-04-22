namespace YepPet.Application.Places;

/// <summary>
/// External city suggestion source (e.g., GeoNames).
/// </summary>
public interface IExternalCitySuggestionProvider
{
    Task<IReadOnlyCollection<PlaceCitySuggestionDto>> SearchCitiesAsync(
        string normalizedQuery,
        int limit,
        CancellationToken cancellationToken = default);
}
