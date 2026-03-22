import { Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { ErrorNotificationsService } from '../../../../core/services/error-notifications.service';
import { SectionHeadingComponent } from '../../../../shared/components/section-heading/section-heading.component';
import { PlaceFiltersComponent } from '../../../places/components/place-filters/place-filters.component';
import { PlaceMapComponent } from '../../../places/components/place-map/place-map.component';
import { PlaceFilters } from '../../../places/models/place.model';
import { PlaceService } from '../../../places/services/place.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login-page',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    SectionHeadingComponent,
    PlaceFiltersComponent,
    PlaceMapComponent
  ],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly notifications = inject(ErrorNotificationsService);
  private readonly placeService = inject(PlaceService);
  private readonly previewFiltersState = signal<PlaceFilters>({
    search: '',
    city: '',
    type: 'hotel',
    pet: 'dogs'
  });

  protected readonly previewFilters = this.previewFiltersState.asReadonly();
  protected readonly previewCities = this.placeService.getAvailableCities();
  protected readonly previewTypes = this.placeService.getAvailableTypes();
  protected readonly samplePlaces = computed(() =>
    this.placeService.getPlaces(this.previewFilters()).slice(0, 4)
  );
  protected readonly loginPreviewRoute = computed(() => {
    const filters = this.previewFilters();
    const queryParams = {
      search: filters.search || null,
      city: filters.city || null,
      type: filters.type || null,
      pet: filters.pet !== 'all' ? filters.pet : null
    };

    return this.router.serializeUrl(
      this.router.createUrlTree(['/places'], {
        queryParams
      })
    );
  });

  protected readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.notify('Revisa el formulari', 'Cal informar un email vàlid i la contrasenya.');

      return;
    }

    const result = this.authService.login(this.form.getRawValue());

    if (!result.ok) {
      this.notifications.notify(
        'Credencials incorrectes',
        'Prova amb admin@admin.adm / Admin123 o user@user.com / Admin123.'
      );

      return;
    }

    this.notifications.notify('Sessió iniciada', `Benvingut/da, ${result.user?.name}.`);

    const redirectTo = this.route.snapshot.queryParamMap.get('redirectTo');
    void this.router.navigateByUrl(redirectTo || this.authService.getPostLoginRoute());
  }

  protected goToPreviewSearch(): void {
    void this.router.navigate(['/login'], {
      queryParams: {
        redirectTo: this.loginPreviewRoute()
      }
    });
  }

  protected updatePreviewFilters(partial: Partial<PlaceFilters>): void {
    this.previewFiltersState.update((current) => ({
      ...current,
      ...partial
    }));
  }
}
