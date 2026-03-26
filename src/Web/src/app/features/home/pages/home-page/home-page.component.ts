import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

import { SiteFooterComponent } from '../../../../core/layout/components/site-footer/site-footer.component';
import { SiteHeaderComponent } from '../../../../core/layout/components/site-header/site-header.component';
import { HomeHeroSectionComponent } from '../../components/home-hero-section/home-hero-section.component';
import { TrendingCitiesSectionComponent } from '../../components/trending-cities-section/trending-cities-section.component';
import { WhyYepPetSectionComponent } from '../../components/why-yeppet-section/why-yeppet-section.component';
import {
  HOME_HERO_FAKE,
  HOME_TRENDING_CITIES_FAKE,
  HOME_WHY_FAKE
} from '../../mock/home-page.fake';

@Component({
  selector: 'app-home-page',
  imports: [
    RouterLink,
    SiteHeaderComponent,
    SiteFooterComponent,
    HomeHeroSectionComponent,
    TrendingCitiesSectionComponent,
    WhyYepPetSectionComponent
  ],
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss'
})
export class HomePageComponent {
  protected readonly heroContent = HOME_HERO_FAKE;
  protected readonly cities = HOME_TRENDING_CITIES_FAKE;
  protected readonly whyContent = HOME_WHY_FAKE;
  protected readonly journeySteps = [
    {
      step: '01',
      title: 'Filtra ràpid',
      copy: 'Entra a places, aplica ciutat, tipus o mascota i obtén context des del primer moment.'
    },
    {
      step: '02',
      title: 'Valida al mapa',
      copy: 'El mode mixt et deixa veure mapa i llistat junts per comparar sense perdre orientació.'
    },
    {
      step: '03',
      title: 'Guarda i reprèn',
      copy: 'Els favorits queden llestos per revisar, reordenar i reprendre la cerca quan vulguis.'
    }
  ];
}
