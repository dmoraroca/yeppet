import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { API_BASE_URL } from '../config/api.config';
import { AUTH_STORE } from '../../features/auth/services/auth-store.token';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith(API_BASE_URL)) {
    return next(req);
  }

  const store = inject(AUTH_STORE);
  const session = store.loadSession();

  if (!session?.accessToken) {
    return next(req);
  }

  return next(req.clone({
    setHeaders: {
      Authorization: `Bearer ${session.accessToken}`
    }
  }));
};
