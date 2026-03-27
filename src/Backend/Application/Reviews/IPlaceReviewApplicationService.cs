namespace YepPet.Application.Reviews;

public interface IPlaceReviewApplicationService
{
    Task<IReadOnlyCollection<PlaceReviewDto>> GetByPlaceAsync(
        Guid placeId,
        bool onlyVisible,
        int take,
        CancellationToken cancellationToken = default);

    Task<Guid> SaveAsync(PlaceReviewUpsertRequest request, CancellationToken cancellationToken = default);
}
