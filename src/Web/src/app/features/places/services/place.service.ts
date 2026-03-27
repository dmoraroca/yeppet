import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { catchError, of } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.config';
import { FavoritesService } from '../../favorites/services/favorites.service';
import { PetFilter, Place, PlaceFilters } from '../models/place.model';
import { PLACE_TYPE_LABELS } from '../mock/places.fake';

const DEFAULT_FILTERS: PlaceFilters = {
  search: '',
  city: '',
  type: '',
  pet: 'all'
};

@Injectable({ providedIn: 'root' })
export class PlaceService {
  private readonly http = inject(HttpClient);
  private readonly favoritesService = inject(FavoritesService);
  private readonly placesState = signal<Place[]>([]);
  private readonly loadedState = signal(false);

  constructor() {
    this.reload();
  }

  readonly hasLoaded = computed(() => this.loadedState());

  getPlaces(filters: Partial<PlaceFilters> = {}): Place[] {
    const safeFilters = { ...DEFAULT_FILTERS, ...filters };
    const search = safeFilters.search.trim().toLowerCase();

    return this.placesState().filter((place) => {
      const matchesSearch =
        search.length === 0 ||
        [place.name, place.city, place.neighborhood, place.shortDescription, ...place.tags]
          .join(' ')
          .toLowerCase()
          .includes(search);

      const matchesCity = !safeFilters.city || place.city === safeFilters.city;
      const matchesType = !safeFilters.type || place.type === safeFilters.type;
      const matchesPet = this.matchesPet(place, safeFilters.pet);

      return matchesSearch && matchesCity && matchesType && matchesPet;
    });
  }

  getPlaceById(placeId: string): Place | undefined {
    return this.placesState().find((place) => place.id === placeId);
  }

  getFavoritePlaces(): Place[] {
    const ids = this.favoritesService.favoriteIds();

    return ids
      .map((id) => this.getPlaceById(id))
      .filter((place): place is Place => place !== undefined);
  }

  getAvailableCities(): string[] {
    return [...new Set(this.placesState().map((place) => place.city))].sort((a, b) =>
      a.localeCompare(b)
    );
  }

  getAvailableTypes(): { value: string; label: string }[] {
    return Object.entries(PLACE_TYPE_LABELS).map(([value, label]) => ({ value, label }));
  }

  getTypeLabel(type: Place['type']): string {
    return PLACE_TYPE_LABELS[type];
  }

  reload(): void {
    this.http
      .get<PlaceApiSummaryDto[]>(`${API_BASE_URL}/places`)
      .pipe(catchError(() => of([])))
      .subscribe((places) => {
        this.placesState.set(places.map((place) => this.toPlace(place)));
        this.loadedState.set(true);
      });
  }

  private matchesPet(place: Place, pet: PetFilter): boolean {
    if (pet === 'dogs') {
      return place.acceptsDogs;
    }

    if (pet === 'cats') {
      return place.acceptsCats;
    }

    return true;
  }

  private toPlace(place: PlaceApiSummaryDto): Place {
    return {
      id: place.id,
      name: place.name,
      city: place.city,
      country: place.country,
      neighborhood: place.neighborhood,
      type: place.type.toLowerCase() as Place['type'],
      shortDescription: place.shortDescription,
      description: place.description,
      imageUrl: place.coverImageUrl,
      acceptsDogs: place.acceptsDogs,
      acceptsCats: place.acceptsCats,
      rating: place.ratingAverage,
      reviewCount: place.reviewCount,
      priceLabel: place.pricingLabel,
      petPolicyLabel: place.petPolicyLabel,
      tags: [...place.tags],
      address: `${place.addressLine1}, ${place.city}`,
      petNotes: place.petPolicyNotes,
      features: [...place.features],
      coordinates: {
        lat: place.latitude,
        lng: place.longitude
      }
    };
  }
}

interface PlaceApiSummaryDto {
  id: string;
  name: string;
  type: string;
  shortDescription: string;
  description: string;
  coverImageUrl: string;
  addressLine1: string;
  city: string;
  country: string;
  neighborhood: string;
  latitude: number;
  longitude: number;
  acceptsDogs: boolean;
  acceptsCats: boolean;
  petPolicyLabel: string;
  petPolicyNotes: string;
  pricingLabel: string;
  ratingAverage: number;
  reviewCount: number;
  tags: string[];
  features: string[];
}
