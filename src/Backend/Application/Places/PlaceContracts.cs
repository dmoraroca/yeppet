namespace YepPet.Application.Places;

public sealed record PlaceSearchRequest(
    string? SearchText,
    string? City,
    string? Type,
    string PetCategory);

public sealed record PlaceUpsertRequest(
    Guid? Id,
    string Name,
    string Type,
    string ShortDescription,
    string Description,
    string CoverImageUrl,
    string AddressLine1,
    string City,
    string Country,
    string Neighborhood,
    decimal Latitude,
    decimal Longitude,
    bool AcceptsDogs,
    bool AcceptsCats,
    string PetPolicyLabel,
    string PetPolicyNotes,
    string PricingLabel,
    decimal RatingAverage,
    int ReviewCount,
    IReadOnlyCollection<string> Tags,
    IReadOnlyCollection<string> Features);

public sealed record PlaceSummaryDto(
    Guid Id,
    string Name,
    string Type,
    string City,
    string Country,
    decimal RatingAverage,
    int ReviewCount,
    IReadOnlyCollection<string> Tags,
    IReadOnlyCollection<string> Features);

public sealed record PlaceDetailDto(
    Guid Id,
    string Name,
    string Type,
    string ShortDescription,
    string Description,
    string CoverImageUrl,
    string AddressLine1,
    string City,
    string Country,
    string Neighborhood,
    decimal Latitude,
    decimal Longitude,
    bool AcceptsDogs,
    bool AcceptsCats,
    string PetPolicyLabel,
    string PetPolicyNotes,
    string PricingLabel,
    decimal RatingAverage,
    int ReviewCount,
    IReadOnlyCollection<string> Tags,
    IReadOnlyCollection<string> Features);
