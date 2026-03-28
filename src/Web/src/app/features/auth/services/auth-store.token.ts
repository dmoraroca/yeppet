import { InjectionToken } from '@angular/core';

import { AuthSession } from '../models/auth-user.model';

export interface AuthStore {
  loadSession(): AuthSession | null;
  saveSession(session: AuthSession | null): void;
}

export const AUTH_STORE = new InjectionToken<AuthStore>('AUTH_STORE');
