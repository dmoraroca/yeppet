import { Inject, Injectable, computed, signal } from '@angular/core';

import { FAVORITES_STORE, FavoritesStore } from './favorites-store.token';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly favoriteIdsState: ReturnType<typeof signal<string[]>>;

  constructor(@Inject(FAVORITES_STORE) private readonly favoritesStore: FavoritesStore) {
    this.favoriteIdsState = signal<string[]>(this.favoritesStore.loadFavoriteIds());
  }

  readonly favoriteIds = computed(() => this.favoriteIdsState());
  readonly count = computed(() => this.favoriteIdsState().length);

  isFavorite(placeId: string): boolean {
    return this.favoriteIdsState().includes(placeId);
  }

  toggle(placeId: string): void {
    const nextIds = this.favoriteIdsState().includes(placeId)
      ? this.favoriteIdsState().filter((id) => id !== placeId)
      : [placeId, ...this.favoriteIdsState().filter((id) => id !== placeId)];

    this.favoriteIdsState.set(nextIds);
    this.favoritesStore.saveFavoriteIds(nextIds);
  }

  clear(): void {
    this.favoriteIdsState.set([]);
    this.favoritesStore.saveFavoriteIds([]);
  }
}
