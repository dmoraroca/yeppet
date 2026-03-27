using Microsoft.AspNetCore.Http.HttpResults;
using YepPet.Application.Places;

namespace YepPet.Api.Endpoints;

internal static class PlaceEndpoints
{
    public static IEndpointRouteBuilder MapPlaceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/places");

        group.MapGet("/", SearchAsync);
        group.MapGet("/cities", GetAvailableCitiesAsync);
        group.MapGet("/{id:guid}", GetByIdAsync);
        group.MapPost("/", SaveAsync);
        group.MapPut("/{id:guid}", UpdateAsync);

        return app;
    }

    private static async Task<Ok<IReadOnlyCollection<PlaceSummaryDto>>> SearchAsync(
        [AsParameters] PlaceSearchQuery query,
        IPlaceApplicationService service,
        CancellationToken cancellationToken)
    {
        var result = await service.SearchAsync(
            new PlaceSearchRequest(query.SearchText, query.City, query.Type, query.PetCategory ?? "All"),
            cancellationToken);

        return TypedResults.Ok(result);
    }

    private static async Task<Ok<IReadOnlyCollection<string>>> GetAvailableCitiesAsync(
        IPlaceApplicationService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetAvailableCitiesAsync(cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<PlaceDetailDto>, NotFound>> GetByIdAsync(
        Guid id,
        IPlaceApplicationService service,
        CancellationToken cancellationToken)
    {
        var place = await service.GetByIdAsync(id, cancellationToken);
        return place is null ? TypedResults.NotFound() : TypedResults.Ok(place);
    }

    private static async Task<Created<Guid>> SaveAsync(
        PlaceUpsertRequest request,
        IPlaceApplicationService service,
        CancellationToken cancellationToken)
    {
        var id = await service.SaveAsync(request, cancellationToken);
        return TypedResults.Created($"/api/places/{id}", id);
    }

    private static async Task<Ok<Guid>> UpdateAsync(
        Guid id,
        PlaceUpsertRequest request,
        IPlaceApplicationService service,
        CancellationToken cancellationToken)
    {
        var resultId = await service.SaveAsync(request with { Id = id }, cancellationToken);
        return TypedResults.Ok(resultId);
    }

    internal sealed record PlaceSearchQuery(
        string? SearchText,
        string? City,
        string? Type,
        string? PetCategory);
}
