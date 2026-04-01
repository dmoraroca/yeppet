import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { ErrorNotificationsService } from '../../../../core/services/error-notifications.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-auth-callback-page',
  template: ''
})
export class AuthCallbackPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly notifications = inject(ErrorNotificationsService);

  constructor() {
    void this.completeAsync();
  }

  private async completeAsync(): Promise<void> {
    const sessionPayload = this.route.snapshot.queryParamMap.get('session');
    const redirectTo = this.route.snapshot.queryParamMap.get('redirectTo');

    if (!sessionPayload) {
      this.notifications.notify('Login social incomplet', 'No s’ha rebut la sessió del proveïdor federat.');
      void this.router.navigateByUrl('/login');
      return;
    }

    try {
      const json = this.decodeBase64Url(sessionPayload);
      const user = this.authService.hydrateFederatedSession(JSON.parse(json) as AuthSessionApiDto);
      this.notifications.notify('Sessió iniciada', `Benvingut/da, ${user.name}.`);
      void this.router.navigateByUrl(redirectTo || this.authService.getPostLoginRoute());
    } catch {
      this.notifications.notify('Login social incomplet', 'No s’ha pogut recuperar la sessió federada.');
      void this.router.navigateByUrl('/login');
    }
  }

  private decodeBase64Url(value: string): string {
    const normalized = value.replace(/-/g, '+').replace(/_/g, '/');
    const padded = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, '=');
    return decodeURIComponent(escape(window.atob(padded)));
  }
}

interface UserApiDto {
  id: string;
  email: string;
  role: string;
  displayName: string;
  city: string;
  country: string;
  bio: string;
  avatarUrl: string | null;
  privacyAccepted: boolean;
  privacyAcceptedAtUtc: string | null;
}

interface AuthSessionApiDto {
  accessToken: string;
  expiresAtUtc: string;
  provider: string;
  user: UserApiDto;
}
