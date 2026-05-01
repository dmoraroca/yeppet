using Zuppeto.Domain.Abstractions;
using Zuppeto.Domain.Reviews;

namespace Zuppeto.Application.Reviews;

internal sealed class PlaceReviewApplicationService(IPlaceReviewRepository reviewRepository) : IPlaceReviewApplicationService
{
    public async Task<IReadOnlyCollection<PlaceReviewDto>> GetByPlaceAsync(
        Guid placeId,
        bool onlyVisible,
        int take,
        CancellationToken cancellationToken = default)
    {
        var reviews = await reviewRepository.GetByPlaceAsync(
            placeId,
            new PlaceReviewQuery(onlyVisible, take),
            cancellationToken);

        return reviews
            .Select(ToDto)
            .ToArray();
    }

    public async Task<Guid> SaveAsync(PlaceReviewUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var reviewId = request.Id ?? Guid.NewGuid();
        var review = new PlaceReview(
            reviewId,
            request.PlaceId,
            request.AuthorUserId,
            request.Score,
            request.Comment,
            DateTimeOffset.UtcNow);

        if (!request.IsVisible)
        {
            review.Hide();
        }

        var existing = await reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (existing is null)
        {
            await reviewRepository.AddAsync(review, cancellationToken);
        }
        else
        {
            await reviewRepository.UpdateAsync(review, cancellationToken);
        }

        return reviewId;
    }

    private static PlaceReviewDto ToDto(PlaceReview review)
    {
        return new PlaceReviewDto(
            review.Id,
            review.PlaceId,
            review.AuthorUserId,
            review.Score,
            review.Comment,
            review.IsVisible,
            review.CreatedAtUtc);
    }
}
