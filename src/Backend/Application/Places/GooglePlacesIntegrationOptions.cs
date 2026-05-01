namespace YepPet.Application.Places;

/// <summary>
/// Application-level settings for Google Places integration (reads the same configuration section as Infrastructure).
/// </summary>
public sealed class GooglePlacesIntegrationOptions
{
    public const string SectionName = "GooglePlaces";

    /// <summary>
    /// Days until advertised Google coordinate cache expiry. Aligns with operational refresh via Places Details using <c>place_id</c>.
    /// </summary>
    public int CoordinateCacheRetentionDays { get; set; } = 30;
}
