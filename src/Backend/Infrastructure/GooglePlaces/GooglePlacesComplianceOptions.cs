namespace Zuppeto.Infrastructure.GooglePlaces;

/// <summary>
/// Operational retention controls for persisted Google Places-derived coordinates (configure per current Maps Platform terms).
/// </summary>
public sealed class GooglePlacesComplianceOptions
{
    public const string SectionName = "GooglePlacesCompliance";

    /// <summary>
    /// When true, expired Google coordinate caches are redacted in storage on a fixed interval.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Minimum interval between purge runs (minutes).
    /// </summary>
    public int RunIntervalMinutes { get; set; } = 360;
}
