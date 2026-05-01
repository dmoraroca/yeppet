using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Zuppeto.Application.Reviews;

namespace Zuppeto.Api.Endpoints;

internal static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reviews");

        group.MapGet("/places/{placeId:guid}", GetByPlaceAsync);
        group.MapPost("/", SaveAsync).RequireAuthorization();
        group.MapPut("/{id:guid}", UpdateAsync).RequireAuthorization();

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

    private static async Task<Results<Created<Guid>, ForbidHttpResult>> SaveAsync(
        ClaimsPrincipal principal,
        PlaceReviewUpsertRequest request,
        IPlaceReviewApplicationService service,
        CancellationToken cancellationToken)
    {
        if (!IsReviewAuthor(principal, request.AuthorUserId))
        {
            return TypedResults.Forbid();
        }

        var reviewId = await service.SaveAsync(request, cancellationToken);
        return TypedResults.Created($"/api/reviews/{reviewId}", reviewId);
    }

    private static async Task<Results<Ok<Guid>, ForbidHttpResult>> UpdateAsync(
        Guid id,
        ClaimsPrincipal principal,
        PlaceReviewUpsertRequest request,
        IPlaceReviewApplicationService service,
        CancellationToken cancellationToken)
    {
        if (!IsReviewAuthor(principal, request.AuthorUserId))
        {
            return TypedResults.Forbid();
        }

        var reviewId = await service.SaveAsync(request with { Id = id }, cancellationToken);
        return TypedResults.Ok(reviewId);
    }

    private static bool IsReviewAuthor(ClaimsPrincipal principal, Guid authorUserId) =>
        principal.GetCurrentUserId() is { } uid && uid == authorUserId;

    internal sealed record ReviewQuery(bool OnlyVisible = true, int Take = 20);
}
