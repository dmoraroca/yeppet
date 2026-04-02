import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { API_BASE_URL } from '../../../core/config/api.config';
import { AuthCredentials, AuthProfileUpdate, AuthProvider, AuthRole, AuthSession, AuthUser } from '../models/auth-user.model';
import { AUTH_STORE, AuthStore } from './auth-store.token';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly sessionState: ReturnType<typeof signal<AuthSession | null>>;

  constructor(@Inject(AUTH_STORE) private readonly authStore: AuthStore) {
    this.sessionState = signal<AuthSession | null>(this.authStore.loadSession());
  }

  readonly currentUser = computed(() => this.sessionState()?.user ?? null);
  readonly isAuthenticated = computed(() => this.sessionState() !== null);
  readonly isAdmin = computed(() => this.sessionState()?.user.role === 'ADMIN');
  readonly role = computed<AuthRole | null>(() => this.sessionState()?.user.role ?? null);
  readonly provider = computed(() => this.sessionState()?.provider ?? null);

  async login(credentials: AuthCredentials): Promise<{ ok: boolean; user?: AuthUser }> {
    try {
      const session = await firstValueFrom(
        this.http.post<AuthSessionApiDto>(`${API_BASE_URL}/auth/login`, {
          email: credentials.email.trim(),
          password: credentials.password.trim()
        })
      );

      const mappedSession = this.toSession(session);
      this.sessionState.set(mappedSession);
      this.authStore.saveSession(mappedSession);

      return { ok: true, user: mappedSession.user };
    } catch {
      return { ok: false };
    }
  }

  async loginWithGoogle(idToken: string): Promise<{ ok: boolean; user?: AuthUser }> {
    try {
      const session = await firstValueFrom(
        this.http.post<AuthSessionApiDto>(`${API_BASE_URL}/auth/google`, {
          idToken
        })
      );

      const mappedSession = this.toSession(session);
      this.sessionState.set(mappedSession);
      this.authStore.saveSession(mappedSession);

      return { ok: true, user: mappedSession.user };
    } catch {
      return { ok: false };
    }
  }

  hydrateFederatedSession(session: AuthSessionApiDto): AuthUser {
    const mappedSession = this.toSession(this.normalizeSession(session));
    this.sessionState.set(mappedSession);
    this.authStore.saveSession(mappedSession);
    return mappedSession.user;
  }

  logout(): void {
    this.sessionState.set(null);
    this.authStore.saveSession(null);
  }

  async updateProfile(update: AuthProfileUpdate): Promise<AuthUser | null> {
    const current = this.currentUser();

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

    this.sessionState.update((session) =>
      session
        ? {
            ...session,
            user: nextUser
          }
        : session
    );
    this.authStore.saveSession(this.sessionState());

    return nextUser;
  }

  async getProviders(): Promise<AuthProvider[]> {
    const providers = await firstValueFrom(this.http.get<AuthProviderApiDto[]>(`${API_BASE_URL}/auth/providers`));
    return providers.map((provider) => ({
      key: provider.key,
      displayName: provider.displayName,
      protocol: provider.protocol,
      configured: provider.configured,
      clientId: provider.clientId ?? null
    }));
  }

  getPostLoginRoute(): string {
    const user = this.currentUser();

    if (!user) {
      return '/login';
    }

    if (user.role === 'ADMIN') {
      return '/permissions';
    }

    return this.requiresProfileCompletion(user) ? '/perfil' : '/';
  }

  getLinkedInStartUrl(redirectTo?: string | null): string {
    const target = redirectTo?.trim();
    return target
      ? `${API_BASE_URL}/auth/linkedin/start?redirectTo=${encodeURIComponent(target)}`
      : `${API_BASE_URL}/auth/linkedin/start`;
  }

  getFacebookStartUrl(redirectTo?: string | null): string {
    const target = redirectTo?.trim();
    return target
      ? `${API_BASE_URL}/auth/facebook/start?redirectTo=${encodeURIComponent(target)}`
      : `${API_BASE_URL}/auth/facebook/start`;
  }

  private toSession(session: AuthSessionApiDto): AuthSession {
    return {
      accessToken: session.accessToken,
      expiresAtUtc: session.expiresAtUtc,
      provider: session.provider,
      user: this.toAuthUser(session.user)
    };
  }

  private normalizeSession(session: AuthSessionApiDto | PascalCaseAuthSessionApiDto): AuthSessionApiDto {
    const candidate = session as Partial<AuthSessionApiDto> & Partial<PascalCaseAuthSessionApiDto>;
    const user = candidate.user ?? candidate.User;

    if (!user) {
      throw new Error('Federated session does not include user payload.');
    }

    return {
      accessToken: candidate.accessToken ?? candidate.AccessToken ?? '',
      expiresAtUtc: candidate.expiresAtUtc ?? candidate.ExpiresAtUtc ?? '',
      provider: candidate.provider ?? candidate.Provider ?? '',
      user: {
        id: this.readUserField(user, 'id', 'Id') ?? '',
        email: this.readUserField(user, 'email', 'Email') ?? '',
        role: this.readUserField(user, 'role', 'Role') ?? '',
        displayName: this.readUserField(user, 'displayName', 'DisplayName') ?? '',
        city: this.readUserField(user, 'city', 'City') ?? '',
        country: this.readUserField(user, 'country', 'Country') ?? '',
        bio: this.readUserField(user, 'bio', 'Bio') ?? '',
        avatarUrl: this.readUserField(user, 'avatarUrl', 'AvatarUrl') ?? null,
        privacyAccepted: this.readUserField(user, 'privacyAccepted', 'PrivacyAccepted') ?? false,
        privacyAcceptedAtUtc: this.readUserField(user, 'privacyAcceptedAtUtc', 'PrivacyAcceptedAtUtc') ?? null
      }
    };
  }

  private readUserField<T>(
    user: UserApiDto | PascalCaseUserApiDto,
    camelKey: keyof UserApiDto,
    pascalKey: keyof PascalCaseUserApiDto
  ): T | undefined {
    const record = user as Record<string, unknown>;
    return (record[camelKey as string] ?? record[pascalKey as string]) as T | undefined;
  }

  private toAuthUser(user: UserApiDto): AuthUser {
    return {
      id: user.id,
      email: user.email,
      name: user.displayName,
      role: user.role.toUpperCase() as AuthRole,
      city: user.city,
      country: user.country,
      bio: user.bio,
      avatarUrl: user.avatarUrl,
      privacyAccepted: user.privacyAccepted
    };
  }

  private requiresProfileCompletion(user: AuthUser): boolean {
    return (
      !user.name.trim() ||
      !user.city.trim() ||
      !user.country.trim() ||
      !user.bio.trim() ||
      !user.privacyAccepted
    );
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

interface PascalCaseUserApiDto {
  Id?: string;
  Email?: string;
  Role?: string;
  DisplayName?: string;
  City?: string;
  Country?: string;
  Bio?: string;
  AvatarUrl?: string | null;
  PrivacyAccepted?: boolean;
  PrivacyAcceptedAtUtc?: string | null;
}

interface PascalCaseAuthSessionApiDto {
  AccessToken?: string;
  ExpiresAtUtc?: string;
  Provider?: string;
  User?: PascalCaseUserApiDto;
}

interface AuthProviderApiDto {
  key: string;
  displayName: string;
  protocol: string;
  configured: boolean;
  clientId?: string | null;
}
