import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, computed, effect, inject, signal } from '@angular/core';
import { catchError, map, of } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.config';
import { AuthService } from '../../auth/services/auth.service';
import { FAVORITES_STORE, FavoritesStore } from './favorites-store.token';

@Injectable({ providedIn: 'root' })
export class FavoritesService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);
  private readonly favoriteIdsState: ReturnType<typeof signal<string[]>>;

  constructor(@Inject(FAVORITES_STORE) private readonly favoritesStore: FavoritesStore) {
    this.favoriteIdsState = signal<string[]>(this.favoritesStore.loadFavoriteIds());

    effect(() => {
      const currentUser = this.authService.currentUser();

      if (!currentUser) {
        this.favoriteIdsState.set([]);
        this.favoritesStore.saveFavoriteIds([]);
        return;
      }

      this.http
        .get<FavoriteListDto>(`${API_BASE_URL}/favorites/${currentUser.id}`)
        .pipe(
          map((favoriteList) => favoriteList.entries.map((entry) => entry.placeId)),
          catchError(() => of([]))
        )
        .subscribe((ids) => {
          this.favoriteIdsState.set(ids);
          this.favoritesStore.saveFavoriteIds(ids);
        });
    });
  }

  readonly favoriteIds = computed(() => this.favoriteIdsState());
  readonly count = computed(() => this.favoriteIdsState().length);

  isFavorite(placeId: string): boolean {
    return this.favoriteIdsState().includes(placeId);
  }

  toggle(placeId: string): void {
    const currentUser = this.authService.currentUser();

    if (!currentUser) {
      return;
    }

    if (this.favoriteIdsState().includes(placeId)) {
      this.http.delete(`${API_BASE_URL}/favorites/${currentUser.id}/places/${placeId}`).subscribe(() => {
        const nextIds = this.favoriteIdsState().filter((id) => id !== placeId);
        this.favoriteIdsState.set(nextIds);
        this.favoritesStore.saveFavoriteIds(nextIds);
      });

      return;
    }

    this.http.post(`${API_BASE_URL}/favorites/${currentUser.id}/places/${placeId}`, {}).subscribe(() => {
      const nextIds = [placeId, ...this.favoriteIdsState().filter((id) => id !== placeId)];
      this.favoriteIdsState.set(nextIds);
      this.favoritesStore.saveFavoriteIds(nextIds);
    });
  }

  clear(): void {
    const currentUser = this.authService.currentUser();

    if (!currentUser) {
      this.favoriteIdsState.set([]);
      this.favoritesStore.saveFavoriteIds([]);
      return;
    }

    const ids = [...this.favoriteIdsState()];

    ids.forEach((placeId) => {
      this.http.delete(`${API_BASE_URL}/favorites/${currentUser.id}/places/${placeId}`).subscribe();
    });

    this.favoriteIdsState.set([]);
    this.favoritesStore.saveFavoriteIds([]);
  }
}

interface FavoriteListDto {
  id: string;
  ownerUserId: string;
  entries: Array<{
    id: string;
    placeId: string;
    savedAtUtc: string;
  }>;
}
