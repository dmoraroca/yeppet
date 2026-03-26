import { InjectionToken } from '@angular/core';

import { AuthUser } from '../models/auth-user.model';

export interface AuthStore {
  loadUsers(): AuthUser[];
  saveUsers(users: AuthUser[]): void;
  loadCurrentUser(): AuthUser | null;
  saveCurrentUser(user: AuthUser | null): void;
}

export const AUTH_STORE = new InjectionToken<AuthStore>('AUTH_STORE');
