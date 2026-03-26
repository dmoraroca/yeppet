import { Injectable } from '@angular/core';

import { FavoritesStore } from '../services/favorites-store.token';

const STORAGE_KEY = 'yeppet-favorite-ids';
const DEFAULT_FAVORITES = ['barcelona-pawtel-gotic', 'berlin-grunhof'];

@Injectable({ providedIn: 'root' })
export class MockFavoritesStoreService implements FavoritesStore {
  loadFavoriteIds(): string[] {
    if (typeof localStorage === 'undefined') {
      return DEFAULT_FAVORITES;
    }

    const raw = localStorage.getItem(STORAGE_KEY);

    if (!raw) {
      return DEFAULT_FAVORITES;
    }

    try {
      const parsed = JSON.parse(raw);

      return Array.isArray(parsed) ? parsed.filter((value): value is string => typeof value === 'string') : [];
    } catch {
      localStorage.removeItem(STORAGE_KEY);

      return DEFAULT_FAVORITES;
    }
  }

  saveFavoriteIds(ids: string[]): void {
    if (typeof localStorage === 'undefined') {
      return;
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify(ids));
  }
}
