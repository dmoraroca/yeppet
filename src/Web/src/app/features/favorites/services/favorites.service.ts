import { Injectable, computed, signal } from '@angular/core';

const STORAGE_KEY = 'yeppet-favorite-ids';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly favoriteIdsState = signal<string[]>(this.restoreFavorites());

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
    this.persistFavorites(nextIds);
  }

  clear(): void {
    this.favoriteIdsState.set([]);
    this.persistFavorites([]);
  }

  private restoreFavorites(): string[] {
    if (typeof localStorage === 'undefined') {
      return this.getDefaultFavorites();
    }

    const raw = localStorage.getItem(STORAGE_KEY);

    if (!raw) {
      return this.getDefaultFavorites();
    }

    try {
      const parsed = JSON.parse(raw);

      return Array.isArray(parsed) ? parsed.filter((value): value is string => typeof value === 'string') : [];
    } catch {
      localStorage.removeItem(STORAGE_KEY);

      return this.getDefaultFavorites();
    }
  }

  private persistFavorites(ids: string[]): void {
    if (typeof localStorage === 'undefined') {
      return;
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify(ids));
  }

  private getDefaultFavorites(): string[] {
    return ['barcelona-pawtel-gotic', 'berlin-grunhof'];
  }
}
