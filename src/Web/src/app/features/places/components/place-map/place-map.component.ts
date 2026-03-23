import {
  AfterViewInit,
  Component,
  ElementRef,
  OnChanges,
  OnDestroy,
  SimpleChanges,
  ViewChild,
  input,
  output
} from '@angular/core';

import { Place } from '../../models/place.model';

type LeafletModule = typeof import('leaflet');
type LeafletMap = import('leaflet').Map;
type LeafletLayerGroup = import('leaflet').LayerGroup;

@Component({
  selector: 'app-place-map',
  templateUrl: './place-map.component.html',
  styleUrl: './place-map.component.scss'
})
export class PlaceMapComponent implements AfterViewInit, OnChanges, OnDestroy {
  @ViewChild('mapContainer') private readonly mapContainer?: ElementRef<HTMLDivElement>;

  readonly places = input.required<Place[]>();
  readonly selectedPlaceId = input<string | null>(null);
  readonly height = input('24rem');
  readonly emptyTitle = input('No hi ha ubicacions per mostrar');
  readonly emptyCopy = input('Ajusta els filtres per veure llocs al mapa.');
  readonly showToolbarActions = input(true);
  readonly placeSelected = output<string>();
  readonly selectionCleared = output<void>();

  private leaflet?: LeafletModule;
  private map?: LeafletMap;
  private markersLayer?: LeafletLayerGroup;
  private readonly markers = new Map<string, import('leaflet').CircleMarker>();

  async ngAfterViewInit(): Promise<void> {
    await this.ensureMap();
    this.renderMap();
  }

  ngOnChanges(_changes: SimpleChanges): void {
    this.renderMap();
  }

  ngOnDestroy(): void {
    this.map?.remove();
  }

  protected get hasPlaces(): boolean {
    return this.places().length > 0;
  }

  protected get placesCountLabel(): string {
    return this.places().length === 1 ? '1 ubicació visible' : `${this.places().length} ubicacions visibles`;
  }

  protected get selectedPlace() {
    const selectedPlaceId = this.selectedPlaceId();

    return selectedPlaceId ? this.places().find((place) => place.id === selectedPlaceId) ?? null : null;
  }

  protected focusAllPlaces(): void {
    this.fitMapToPlaces();
  }

  protected clearSelection(): void {
    this.selectionCleared.emit();
    this.fitMapToPlaces();
  }

  private async ensureMap(): Promise<void> {
    if (this.map || !this.mapContainer?.nativeElement || !this.hasPlaces) {
      return;
    }

    this.leaflet = await import('leaflet');
    this.map = this.leaflet.map(this.mapContainer.nativeElement, {
      zoomControl: true,
      scrollWheelZoom: false
    });

    this.leaflet
      .tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '&copy; OpenStreetMap contributors'
      })
      .addTo(this.map);

    this.markersLayer = this.leaflet.layerGroup().addTo(this.map);
  }

  private renderMap(): void {
    if (!this.hasPlaces) {
      this.markersLayer?.clearLayers();
      return;
    }

    void this.ensureMap().then(() => {
      if (!this.leaflet || !this.map || !this.markersLayer) {
        return;
      }

      this.markersLayer.clearLayers();
      this.markers.clear();
      const bounds = this.leaflet.latLngBounds([]);
      const selectedPlaceId = this.selectedPlaceId();

      for (const place of this.places()) {
        const isSelected = place.id === selectedPlaceId;
        const marker = this.leaflet.circleMarker([place.coordinates.lat, place.coordinates.lng], {
          radius: isSelected ? 11 : 8,
          weight: isSelected ? 3 : 2,
          color: isSelected ? '#065f46' : '#0f766e',
          fillColor: isSelected ? '#2dd4bf' : '#99f6e4',
          fillOpacity: isSelected ? 0.95 : 0.85
        });

        marker.bindPopup(
          `
            <div class="place-map__popup">
              <p class="place-map__popup-type">${place.type}</p>
              <strong>${place.name}</strong>
              <span>${place.city}, ${place.country}</span>
              <span>${place.address}</span>
              <span>Valoració ${place.rating}</span>
            </div>
          `,
          {
            className: 'place-map__popup-shell'
          }
        );
        marker.on('click', () => this.placeSelected.emit(place.id));
        marker.addTo(this.markersLayer);
        this.markers.set(place.id, marker);
        bounds.extend([place.coordinates.lat, place.coordinates.lng]);
      }

      if (selectedPlaceId) {
        const selectedPlace = this.places().find((place) => place.id === selectedPlaceId);

        if (selectedPlace) {
          this.map.setView([selectedPlace.coordinates.lat, selectedPlace.coordinates.lng], 15);
          this.markers.get(selectedPlace.id)?.openPopup();
          return;
        }
      }

      this.fitMapToPlaces(bounds);
    });
  }

  private fitMapToPlaces(existingBounds?: import('leaflet').LatLngBounds): void {
    if (!this.leaflet || !this.map || !this.hasPlaces) {
      return;
    }

    const bounds =
      existingBounds ??
      this.places().reduce((accumulator, place) => {
        accumulator.extend([place.coordinates.lat, place.coordinates.lng]);

        return accumulator;
      }, this.leaflet.latLngBounds([]));

    this.map.fitBounds(bounds, {
      padding: [28, 28],
      maxZoom: this.places().length === 1 ? 15 : 13
    });
  }
}
