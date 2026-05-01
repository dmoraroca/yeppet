import { Injectable } from '@angular/core';

import { AuthSession } from '../models/auth-user.model';
import { AuthStore } from './auth-store.token';

const STORAGE_KEY = 'zuppeto-auth-session';

@Injectable({ providedIn: 'root' })
export class BrowserAuthStoreService implements AuthStore {
  loadSession(): AuthSession | null {
    if (typeof localStorage === 'undefined') {
      return null;
    }

    const raw = localStorage.getItem(STORAGE_KEY);

    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as AuthSession;
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }

  saveSession(session: AuthSession | null): void {
    if (typeof localStorage === 'undefined') {
      return;
    }

    if (!session) {
      localStorage.removeItem(STORAGE_KEY);
      return;
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
  }
}
