import { Injectable } from '@angular/core';

import { AUTH_USERS_FAKE } from './auth-users.fake';
import { AuthUser } from '../models/auth-user.model';
import { AuthStore } from '../services/auth-store.token';

const STORAGE_KEY = 'yeppet-auth-user';

@Injectable({ providedIn: 'root' })
export class MockAuthStoreService implements AuthStore {
  private usersState = AUTH_USERS_FAKE;

  loadUsers(): AuthUser[] {
    return this.usersState;
  }

  saveUsers(users: AuthUser[]): void {
    this.usersState = users;
  }

  loadCurrentUser(): AuthUser | null {
    if (typeof localStorage === 'undefined') {
      return null;
    }

    const raw = localStorage.getItem(STORAGE_KEY);

    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as AuthUser;
    } catch {
      localStorage.removeItem(STORAGE_KEY);

      return null;
    }
  }

  saveCurrentUser(user: AuthUser | null): void {
    if (typeof localStorage === 'undefined') {
      return;
    }

    if (!user) {
      localStorage.removeItem(STORAGE_KEY);

      return;
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
  }
}
