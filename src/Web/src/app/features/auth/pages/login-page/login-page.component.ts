import { AfterViewInit, Component, ElementRef, ViewChild, computed, effect, inject, signal } from '@angular/core';
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
export class LoginPageComponent implements AfterViewInit {
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
  protected readonly authProviders = signal<string[]>([]);
  protected readonly googleProvider = signal<{ clientId: string } | null>(null);
  protected readonly googleButtonVisible = signal(false);
  protected readonly previewCities = computed(() => this.placeService.getAvailableCities());
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

  @ViewChild('loginSubmitButton') private loginSubmitButton?: ElementRef<HTMLButtonElement>;
  @ViewChild('googleButtonHost') private googleButtonHost?: ElementRef<HTMLDivElement>;
  private googleButtonRendered = false;
  private googleScriptPromise: Promise<void> | null = null;

  constructor() {
    effect(() => {
      this.googleProvider();
      queueMicrotask(() => {
        void this.tryRenderGoogleButtonAsync();
      });
    });

    void this.loadProvidersAsync();
  }

  ngAfterViewInit(): void {
    queueMicrotask(() => {
      void this.tryRenderGoogleButtonAsync();
    });
  }

  protected async submit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.notify('Revisa el formulari', 'Cal informar un email vàlid i la contrasenya.');

      return;
    }

    const result = await this.authService.login(this.form.getRawValue());

    if (!result.ok) {
      this.notifications.notify('Credencials incorrectes', 'Prova amb admin@admin.adm / Admin123 o user@user.com / Admin123.');

      return;
    }

    this.notifications.notify('Sessió iniciada', `Benvingut/da, ${result.user?.name}.`);

    const redirectTo = this.route.snapshot.queryParamMap.get('redirectTo');
    void this.router.navigateByUrl(redirectTo || this.authService.getPostLoginRoute());
  }

  private async loadProvidersAsync(): Promise<void> {
    try {
      const providers = await this.authService.getProviders();
      this.authProviders.set(
        providers.filter((provider) => provider.key !== 'password').map((provider) => provider.displayName)
      );
      const google = providers.find((provider) => provider.key === 'google' && provider.configured && provider.clientId);
      this.googleProvider.set(google?.clientId ? { clientId: google.clientId } : null);
      void this.tryRenderGoogleButtonAsync();
    } catch {
      this.authProviders.set(['Google', 'LinkedIn', 'Facebook']);
      this.googleProvider.set(null);
    }
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

  private async tryRenderGoogleButtonAsync(): Promise<void> {
    const provider = this.googleProvider();
    const host = this.googleButtonHost?.nativeElement;

    if (!provider || !host || this.googleButtonRendered) {
      return;
    }

    await this.loadGoogleScriptAsync();

    if (!window.google?.accounts?.id) {
      return;
    }

    host.innerHTML = '';
    const submitWidth = this.loginSubmitButton?.nativeElement.getBoundingClientRect().width ?? 0;
    const fallbackWidth = host.parentElement?.getBoundingClientRect().width ?? host.getBoundingClientRect().width ?? host.clientWidth ?? 320;
    const width = Math.max(280, Math.round(submitWidth || fallbackWidth));
    window.google.accounts.id.initialize({
      client_id: provider.clientId,
      callback: ({ credential }) => {
        void this.handleGoogleCredentialAsync(credential);
      }
    });
    window.google.accounts.id.renderButton(host, {
      theme: 'outline',
      size: 'large',
      shape: 'pill',
      text: 'signin_with',
      width
    });
    this.googleButtonRendered = true;
    this.googleButtonVisible.set(true);
  }

  private loadGoogleScriptAsync(): Promise<void> {
    if (window.google?.accounts?.id) {
      return Promise.resolve();
    }

    if (this.googleScriptPromise) {
      return this.googleScriptPromise;
    }

    this.googleScriptPromise = new Promise<void>((resolve, reject) => {
      const existing = document.querySelector<HTMLScriptElement>('script[data-google-identity]');

      if (existing) {
        existing.addEventListener('load', () => resolve(), { once: true });
        existing.addEventListener('error', () => reject(new Error('No s’ha pogut carregar Google Identity Services.')), {
          once: true
        });
        return;
      }

      const script = document.createElement('script');
      script.src = 'https://accounts.google.com/gsi/client';
      script.async = true;
      script.defer = true;
      script.setAttribute('data-google-identity', 'true');
      script.onload = () => resolve();
      script.onerror = () => reject(new Error('No s’ha pogut carregar Google Identity Services.'));
      document.head.appendChild(script);
    });

    return this.googleScriptPromise;
  }

  private async handleGoogleCredentialAsync(idToken: string): Promise<void> {
    const result = await this.authService.loginWithGoogle(idToken);

    if (!result.ok) {
      this.notifications.notify(
        'Google no disponible',
        'Configura un Google Client ID vàlid i verifica la federació per activar aquest accés.'
      );
      return;
    }

    this.notifications.notify('Sessió iniciada', `Benvingut/da, ${result.user?.name}.`);
    const redirectTo = this.route.snapshot.queryParamMap.get('redirectTo');
    void this.router.navigateByUrl(redirectTo || this.authService.getPostLoginRoute());
  }
}
