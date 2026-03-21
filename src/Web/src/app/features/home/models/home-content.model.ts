export interface HomeCity {
  name: string;
  country: string;
  vibe: string;
}

export interface HomeHeroFeaturedPlace {
  id: string;
  name: string;
  city: string;
  badge: string;
}

export interface HomeHeroContent {
  eyebrow: string;
  titleStart: string;
  titleHighlight: string;
  titleEnd: string;
  copy: string;
  chips: string[];
  quickMatchTitle: string;
  quickMatchCopy: string;
  futureReadyTitle: string;
  futureReadyCopy: string;
  featuredPlaces: HomeHeroFeaturedPlace[];
}

export interface HomeWhyContent {
  eyebrow: string;
  title: string;
  reasons: string[];
}
