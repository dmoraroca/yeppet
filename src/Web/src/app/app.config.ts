import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { MockAuthStoreService } from './features/auth/mock/mock-auth-store.service';
import { AUTH_STORE } from './features/auth/services/auth-store.token';
import { MockFavoritesStoreService } from './features/favorites/mock/mock-favorites-store.service';
import { FAVORITES_STORE } from './features/favorites/services/favorites-store.token';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([errorInterceptor])),
    {
      provide: FAVORITES_STORE,
      useExisting: MockFavoritesStoreService
    },
    {
      provide: AUTH_STORE,
      useExisting: MockAuthStoreService
    },
    provideRouter(
      routes,
      withInMemoryScrolling({
        anchorScrolling: 'enabled',
        scrollPositionRestoration: 'enabled'
      })
    )
  ]
};
