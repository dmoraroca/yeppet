namespace YepPet.Application.Places;

public static class PlaceCitySuggestionFormatter
{
    public static string BuildDisplayLabel(string city, string country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return city.Trim();
        }

        return $"{city.Trim()} ({country.Trim()})";
    }
}
