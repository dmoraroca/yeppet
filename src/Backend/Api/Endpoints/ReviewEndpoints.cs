using Microsoft.AspNetCore.Http.HttpResults;
using YepPet.Application.Reviews;

namespace YepPet.Api.Endpoints;

internal static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reviews");

        group.MapGet("/places/{placeId:guid}", GetByPlaceAsync);
        group.MapPost("/", SaveAsync);
        group.MapPut("/{id:guid}", UpdateAsync);

        return app;
    }

    private static async Task<Ok<IReadOnlyCollection<PlaceReviewDto>>> GetByPlaceAsync(
        Guid placeId,
        [AsParameters] ReviewQuery query,
        IPlaceReviewApplicationService service,
        CancellationToken cancellationToken)
    {
        var result = await service.GetByPlaceAsync(placeId, query.OnlyVisible, query.Take, cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Created<Guid>> SaveAsync(
        PlaceReviewUpsertRequest request,
        IPlaceReviewApplicationService service,
        CancellationToken cancellationToken)
    {
        var reviewId = await service.SaveAsync(request, cancellationToken);
        return TypedResults.Created($"/api/reviews/{reviewId}", reviewId);
    }

    private static async Task<Ok<Guid>> UpdateAsync(
        Guid id,
        PlaceReviewUpsertRequest request,
        IPlaceReviewApplicationService service,
        CancellationToken cancellationToken)
    {
        var reviewId = await service.SaveAsync(request with { Id = id }, cancellationToken);
        return TypedResults.Ok(reviewId);
    }

    internal sealed record ReviewQuery(bool OnlyVisible = true, int Take = 20);
}
