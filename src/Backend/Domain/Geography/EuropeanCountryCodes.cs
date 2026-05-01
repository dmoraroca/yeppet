namespace Zuppeto.Domain.Geography;

/// <summary>
/// ISO 3166-1 alpha-2 codes for countries in the geographic Europe region, aligned with
/// GeoNames <c>continentCode=EU</c> and internal country catalog scoping.
/// </summary>
public static class EuropeanCountryCodes
{
    public static bool IsEuropean(string? iso2)
    {
        if (string.IsNullOrWhiteSpace(iso2))
        {
            return false;
        }

        return Iso3166Alpha2.Contains(iso2.Trim().ToUpperInvariant());
    }

    public static IReadOnlyCollection<string> Iso3166Alpha2 { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "AD", "AL", "AT", "AX", "BA", "BE", "BG", "BY", "CH", "CY", "CZ", "DE", "DK", "EE", "ES", "FI", "FO", "FR",
        "GB", "GG", "GI", "GR", "HR", "HU", "IE", "IM", "IS", "IT", "JE", "LI", "LT", "LU", "LV", "MC", "MD", "ME",
        "MK", "MT", "NL", "NO", "PL", "PT", "RO", "RS", "RU", "SE", "SI", "SJ", "SK", "SM", "TR", "UA", "VA", "XK"
    };
}
