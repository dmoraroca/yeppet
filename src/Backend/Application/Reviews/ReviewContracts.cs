namespace Zuppeto.Application.Reviews;

public sealed record PlaceReviewDto(
    Guid Id,
    Guid PlaceId,
    Guid AuthorUserId,
    int Score,
    string Comment,
    bool IsVisible,
    DateTimeOffset CreatedAtUtc);

public sealed record PlaceReviewUpsertRequest(
    Guid? Id,
    Guid PlaceId,
    Guid AuthorUserId,
    int Score,
    string Comment,
    bool IsVisible);
