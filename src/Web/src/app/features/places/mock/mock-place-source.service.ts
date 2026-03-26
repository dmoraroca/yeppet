import { Injectable } from '@angular/core';

import { PlaceSource } from '../services/place-source.token';
import { PLACE_TYPE_LABELS, PLACES_FAKE } from './places.fake';

@Injectable({ providedIn: 'root' })
export class MockPlaceSourceService implements PlaceSource {
  getAllPlaces() {
    return PLACES_FAKE;
  }

  getTypeLabels() {
    return PLACE_TYPE_LABELS;
  }
}
