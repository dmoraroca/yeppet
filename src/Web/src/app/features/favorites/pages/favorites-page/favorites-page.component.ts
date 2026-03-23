import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { SiteFooterComponent } from '../../../../core/layout/components/site-footer/site-footer.component';
import { SiteHeaderComponent } from '../../../../core/layout/components/site-header/site-header.component';
import { SectionHeadingComponent } from '../../../../shared/components/section-heading/section-heading.component';
import { PlaceCardComponent } from '../../../places/components/place-card/place-card.component';
import { PlaceService } from '../../../places/services/place.service';
import { FavoritesService } from '../../services/favorites.service';

@Component({
  selector: 'app-favorites-page',
  imports: [
    RouterLink,
    SiteHeaderComponent,
    SiteFooterComponent,
    SectionHeadingComponent,
    PlaceCardComponent
  ],
  templateUrl: './favorites-page.component.html',
  styleUrl: './favorites-page.component.scss'
})
export class FavoritesPageComponent {
  private readonly placeService = inject(PlaceService);
  private readonly favoritesService = inject(FavoritesService);

  protected readonly places = computed(() => this.placeService.getFavoritePlaces());
  protected readonly favoritesCount = this.favoritesService.count;
  protected readonly latestFavorite = computed(() => this.places()[0] ?? null);
  protected readonly favoriteCities = computed(() =>
    [...new Set(this.places().map((place) => place.city))].sort((left, right) => left.localeCompare(right))
  );
  protected readonly favoriteTypes = computed(() =>
    [...new Set(this.places().map((place) => this.getTypeLabel(place.type)))].sort((left, right) =>
      left.localeCompare(right)
    )
  );

  protected toggleFavorite(placeId: string): void {
    this.favoritesService.toggle(placeId);
  }

  protected clearFavorites(): void {
    this.favoritesService.clear();
  }

  protected isFavorite(placeId: string): boolean {
    return this.favoritesService.isFavorite(placeId);
  }

  protected getTypeLabel(type: string): string {
    return this.placeService.getTypeLabel(type as never);
  }
}
