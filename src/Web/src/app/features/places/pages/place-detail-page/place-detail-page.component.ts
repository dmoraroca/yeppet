import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';

import { SiteFooterComponent } from '../../../../core/layout/components/site-footer/site-footer.component';
import { SiteHeaderComponent } from '../../../../core/layout/components/site-header/site-header.component';
import { FavoriteToggleButtonComponent } from '../../../../shared/components/favorite-toggle-button/favorite-toggle-button.component';
import { FavoritesService } from '../../../favorites/services/favorites.service';
import { PlaceMapComponent } from '../../components/place-map/place-map.component';
import { PlaceService } from '../../services/place.service';

@Component({
  selector: 'app-place-detail-page',
  imports: [
    RouterLink,
    SiteHeaderComponent,
    SiteFooterComponent,
    FavoriteToggleButtonComponent,
    PlaceMapComponent
  ],
  templateUrl: './place-detail-page.component.html',
  styleUrl: './place-detail-page.component.scss'
})
export class PlaceDetailPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly placeService = inject(PlaceService);
  private readonly favoritesService = inject(FavoritesService);
  private readonly params = toSignal(this.route.paramMap, {
    initialValue: this.route.snapshot.paramMap
  });
  private readonly queryParams = toSignal(this.route.queryParamMap, {
    initialValue: this.route.snapshot.queryParamMap
  });

  protected readonly place = computed(() =>
    this.placeService.getPlaceById(this.params().get('id') ?? '')
  );
  protected readonly relatedPlaces = computed(() => {
    const currentPlace = this.place();

    if (!currentPlace) {
      return [];
    }

    return this.placeService
      .getPlaces({ city: currentPlace.city })
      .filter((place) => place.id !== currentPlace.id)
      .slice(0, 3);
  });
  protected readonly petAccessLabel = computed(() => {
    const currentPlace = this.place();

    if (!currentPlace) {
      return '';
    }

    if (currentPlace.acceptsDogs && currentPlace.acceptsCats) {
      return 'Accepta gossos i gats';
    }

    if (currentPlace.acceptsDogs) {
      return 'Especialment còmode per a gossos';
    }

    if (currentPlace.acceptsCats) {
      return 'Especialment còmode per a gats';
    }

    return 'Accés per mascotes limitat';
  });
  protected readonly visitContext = computed(() => {
    const currentPlace = this.place();

    if (!currentPlace) {
      return '';
    }

    if (currentPlace.type === 'hotel' || currentPlace.type === 'apartment') {
      return 'Bona opció si vols resoldre estada i confort pet-friendly en un sol punt.';
    }

    if (currentPlace.type === 'park') {
      return 'Especialment útil com a parada ràpida, descans o passeig durant el dia.';
    }

    return 'Bona opció per encaixar-la dins d’un recorregut urbà amb la mascota sense complicar-te.';
  });
  protected readonly backToPlacesQueryParams = computed(() => {
    const currentPlace = this.place();

    return currentPlace ? { city: currentPlace.city } : {};
  });
  protected readonly cameFromMap = computed(() => this.queryParams().get('fromMap') === 'true');

  protected toggleFavorite(placeId: string): void {
    this.favoritesService.toggle(placeId);
  }

  protected isFavorite(placeId: string): boolean {
    return this.favoritesService.isFavorite(placeId);
  }

  protected getTypeLabel(type: string): string {
    return this.placeService.getTypeLabel(type as never);
  }

  protected getPetMatchSummary(): string {
    const currentPlace = this.place();

    if (!currentPlace) {
      return '';
    }

    if (currentPlace.acceptsDogs && currentPlace.acceptsCats) {
      return 'Compatible amb gossos i gats';
    }

    if (currentPlace.acceptsDogs) {
      return 'Pensat sobretot per a persones que es mouen amb gos';
    }

    if (currentPlace.acceptsCats) {
      return 'Més adequat per a estades amb gat';
    }

    return 'Cal validar el cas concret abans d’anar-hi amb mascota';
  }

  protected get placeAsArray() {
    return this.place() ? [this.place()!] : [];
  }
}
