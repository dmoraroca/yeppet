import { Place } from '../../places/models/place.model';

export type FavoriteReviewSort = 'recent' | 'rating' | 'name';

export function sortPlacesForFavoriteReview(places: Place[], sort: FavoriteReviewSort): Place[] {
  if (sort === 'rating') {
    return [...places].sort((left, right) => right.rating - left.rating);
  }

  if (sort === 'name') {
    return [...places].sort((left, right) => left.name.localeCompare(right.name));
  }

  return places;
}
