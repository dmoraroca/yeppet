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
  readonly placeSelected = output<string>();

  private leaflet?: LeafletModule;
  private map?: LeafletMap;
  private markersLayer?: LeafletLayerGroup;

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
          `<strong>${place.name}</strong><br>${place.city}, ${place.country}<br>${place.address}`
        );
        marker.on('click', () => this.placeSelected.emit(place.id));
        marker.addTo(this.markersLayer);
        bounds.extend([place.coordinates.lat, place.coordinates.lng]);
      }

      if (selectedPlaceId) {
        const selectedPlace = this.places().find((place) => place.id === selectedPlaceId);

        if (selectedPlace) {
          this.map.setView([selectedPlace.coordinates.lat, selectedPlace.coordinates.lng], 15);
          return;
        }
      }

      this.map.fitBounds(bounds, {
        padding: [28, 28],
        maxZoom: this.places().length === 1 ? 15 : 13
      });
    });
  }
}
