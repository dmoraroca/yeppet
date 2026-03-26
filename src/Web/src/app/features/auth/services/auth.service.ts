import { Inject, Injectable, computed, signal } from '@angular/core';
import { AuthCredentials, AuthProfileUpdate, AuthRole, AuthUser } from '../models/auth-user.model';
import { AUTH_STORE, AuthStore } from './auth-store.token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly usersState: ReturnType<typeof signal<AuthUser[]>>;
  private readonly currentUserState: ReturnType<typeof signal<AuthUser | null>>;

  constructor(@Inject(AUTH_STORE) private readonly authStore: AuthStore) {
    this.usersState = signal<AuthUser[]>(this.authStore.loadUsers());
    this.currentUserState = signal<AuthUser | null>(this.authStore.loadCurrentUser());
  }

  readonly currentUser = computed(() => this.currentUserState());
  readonly isAuthenticated = computed(() => this.currentUserState() !== null);
  readonly isAdmin = computed(() => this.currentUserState()?.role === 'ADMIN');
  readonly role = computed<AuthRole | null>(() => this.currentUserState()?.role ?? null);

  login(credentials: AuthCredentials): { ok: boolean; user?: AuthUser } {
    const email = credentials.email.trim().toLowerCase();
    const password = credentials.password.trim();
    const user = this.usersState().find(
      (candidate) => candidate.email.toLowerCase() === email && candidate.password === password
    );

    if (!user) {
      return { ok: false };
    }

    this.currentUserState.set(user);
    this.authStore.saveCurrentUser(user);

    return { ok: true, user };
  }

  logout(): void {
    this.currentUserState.set(null);
    this.authStore.saveCurrentUser(null);
  }

  updateProfile(update: AuthProfileUpdate): AuthUser | null {
    const current = this.currentUserState();

    if (!current) {
      return null;
    }

    const nextUser: AuthUser = {
      ...current,
      ...update
    };

    this.usersState.update((users) => users.map((user) => (user.id === current.id ? nextUser : user)));
    this.authStore.saveUsers(this.usersState());
    this.currentUserState.set(nextUser);
    this.authStore.saveCurrentUser(nextUser);

    return nextUser;
  }

  getPostLoginRoute(): string {
    return this.isAdmin() ? '/permissions' : '/perfil';
  }
}
