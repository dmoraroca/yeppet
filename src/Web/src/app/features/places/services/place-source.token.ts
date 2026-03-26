import { InjectionToken } from '@angular/core';

import { Place, PlaceType } from '../models/place.model';

export interface PlaceSource {
  getAllPlaces(): Place[];
  getTypeLabels(): Record<PlaceType, string>;
}

export const PLACE_SOURCE = new InjectionToken<PlaceSource>('PLACE_SOURCE');
