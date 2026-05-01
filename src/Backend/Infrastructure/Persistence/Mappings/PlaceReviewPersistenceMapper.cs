using Zuppeto.Domain.Reviews;
using Zuppeto.Infrastructure.Persistence.Entities;

namespace Zuppeto.Infrastructure.Persistence.Mappings;

internal static class PlaceReviewPersistenceMapper
{
    public static PlaceReview ToDomain(PlaceReviewRecord record)
    {
        var review = new PlaceReview(
            record.Id,
            record.PlaceId,
            record.AuthorUserId,
            record.Score,
            record.Comment,
            record.CreatedAtUtc);

        if (!record.IsVisible)
        {
            review.Hide();
        }

        return review;
    }

    public static PlaceReviewRecord ToRecord(PlaceReview review)
    {
        var record = new PlaceReviewRecord();
        Apply(review, record);
        return record;
    }

    public static void Apply(PlaceReview review, PlaceReviewRecord record)
    {
        record.Id = review.Id;
        record.PlaceId = review.PlaceId;
        record.AuthorUserId = review.AuthorUserId;
        record.Score = review.Score;
        record.Comment = review.Comment;
        record.IsVisible = review.IsVisible;
        record.CreatedAtUtc = review.CreatedAtUtc;
    }
}
