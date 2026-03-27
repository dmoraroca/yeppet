import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.config';
import { AuthCredentials, AuthProfileUpdate, AuthRole, AuthUser } from '../models/auth-user.model';
import { AUTH_STORE, AuthStore } from './auth-store.token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly usersState: ReturnType<typeof signal<AuthUser[]>>;
  private readonly currentUserState: ReturnType<typeof signal<AuthUser | null>>;

  constructor(@Inject(AUTH_STORE) private readonly authStore: AuthStore) {
    this.usersState = signal<AuthUser[]>(this.authStore.loadUsers());
    this.currentUserState = signal<AuthUser | null>(this.authStore.loadCurrentUser());
    void this.ensureCurrentUserIsBackedAsync();
  }

  readonly currentUser = computed(() => this.currentUserState());
  readonly isAuthenticated = computed(() => this.currentUserState() !== null);
  readonly isAdmin = computed(() => this.currentUserState()?.role === 'ADMIN');
  readonly role = computed<AuthRole | null>(() => this.currentUserState()?.role ?? null);

  async login(credentials: AuthCredentials): Promise<{ ok: boolean; user?: AuthUser }> {
    const email = credentials.email.trim().toLowerCase();
    const password = credentials.password.trim();
    const user = this.usersState().find(
      (candidate) => candidate.email.toLowerCase() === email && candidate.password === password
    );

    if (!user) {
      return { ok: false };
    }

    const backendUser = await this.syncUserWithBackend(user);

    this.usersState.update((users) =>
      users.map((candidate) => (candidate.email.toLowerCase() === email ? backendUser : candidate))
    );
    this.authStore.saveUsers(this.usersState());
    this.currentUserState.set(backendUser);
    this.authStore.saveCurrentUser(backendUser);

    return { ok: true, user: backendUser };
  }

  logout(): void {
    this.currentUserState.set(null);
    this.authStore.saveCurrentUser(null);
  }

  async updateProfile(update: AuthProfileUpdate): Promise<AuthUser | null> {
    const current = this.currentUserState();

    if (!current) {
      return null;
    }

    const nextUser: AuthUser = {
      ...current,
      ...update
    };

    await firstValueFrom(
      this.http.put<void>(`${API_BASE_URL}/users/${current.id}/profile`, {
        id: current.id,
        displayName: update.name,
        city: update.city,
        country: update.country,
        bio: update.bio,
        avatarUrl: update.avatarUrl,
        privacyAccepted: update.privacyAccepted,
        privacyAcceptedAtUtc: update.privacyAccepted ? new Date().toISOString() : null
      })
    );

    this.usersState.update((users) => users.map((user) => (user.id === current.id ? nextUser : user)));
    this.authStore.saveUsers(this.usersState());
    this.currentUserState.set(nextUser);
    this.authStore.saveCurrentUser(nextUser);

    return nextUser;
  }

  getPostLoginRoute(): string {
    return this.isAdmin() ? '/permissions' : '/perfil';
  }

  private async ensureCurrentUserIsBackedAsync(): Promise<void> {
    const currentUser = this.currentUserState();

    if (!currentUser) {
      return;
    }

    const backendUser = await this.syncUserWithBackend(currentUser);
    this.usersState.update((users) =>
      users.map((user) => (user.email.toLowerCase() === backendUser.email.toLowerCase() ? backendUser : user))
    );
    this.authStore.saveUsers(this.usersState());
    this.currentUserState.set(backendUser);
    this.authStore.saveCurrentUser(backendUser);
  }

  private async syncUserWithBackend(user: AuthUser): Promise<AuthUser> {
    try {
      const backendUser = await firstValueFrom(
        this.http.get<UserApiDto>(`${API_BASE_URL}/users/by-email/${encodeURIComponent(user.email)}`)
      );

      return this.toAuthUser(backendUser, user.password);
    } catch {
      const createdUserId = await firstValueFrom(
        this.http.post<string>(`${API_BASE_URL}/users/`, {
          email: user.email,
          passwordHash: user.password,
          role: user.role === 'ADMIN' ? 'Admin' : 'User',
          displayName: user.name,
          city: user.city,
          country: user.country,
          bio: user.bio,
          avatarUrl: user.avatarUrl,
          privacyAccepted: user.privacyAccepted,
          privacyAcceptedAtUtc: user.privacyAccepted ? new Date().toISOString() : null
        })
      );

      const createdUser = await firstValueFrom(this.http.get<UserApiDto>(`${API_BASE_URL}/users/${createdUserId}`));
      return this.toAuthUser(createdUser, user.password);
    }
  }

  private toAuthUser(user: UserApiDto, password: string): AuthUser {
    return {
      id: user.id,
      email: user.email,
      password,
      name: user.displayName,
      role: user.role.toUpperCase() as AuthRole,
      city: user.city,
      country: user.country,
      bio: user.bio,
      avatarUrl: user.avatarUrl,
      privacyAccepted: user.privacyAccepted
    };
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
