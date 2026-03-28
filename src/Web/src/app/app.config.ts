import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { BrowserAuthStoreService } from './features/auth/services/browser-auth-store.service';
import { AUTH_STORE } from './features/auth/services/auth-store.token';
import { MockFavoritesStoreService } from './features/favorites/mock/mock-favorites-store.service';
import { FAVORITES_STORE } from './features/favorites/services/favorites-store.token';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    {
      provide: FAVORITES_STORE,
      useExisting: MockFavoritesStoreService
    },
    {
      provide: AUTH_STORE,
      useExisting: BrowserAuthStoreService
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
