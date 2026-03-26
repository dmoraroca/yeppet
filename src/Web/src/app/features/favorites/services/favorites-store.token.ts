import { InjectionToken } from '@angular/core';

export interface FavoritesStore {
  loadFavoriteIds(): string[];
  saveFavoriteIds(ids: string[]): void;
}

export const FAVORITES_STORE = new InjectionToken<FavoritesStore>('FAVORITES_STORE');
